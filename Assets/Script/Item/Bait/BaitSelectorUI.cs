using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaitSelectorUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private GameObject baitSelectorRoot;       // 미끼 UI 전체를 감싸는 부모 오브젝트
    [SerializeField] private Transform playerTransform;         // 플레이어 위치 (기준점)
    [SerializeField] private GameObject baitSlotUIPrefab;       // 미끼 아이콘을 표시할 작은 동그라미 UI 프리팹
    [SerializeField] private Transform baitSlotsParent;         // 미끼 슬롯들이 생성될 부모 컨테이너
    [SerializeField] private RectTransform selectionIndicator;  // 선택된 미끼를 표시하는 빨간 원 테두리 UI

    [Header("배치 설정")]
    [SerializeField] private float radius = 100f;               // 플레이어 중심부터 아이콘까지의 거리 (UI 캔버스 기준 픽셀)
    [SerializeField] private float openDuration = 0.2f;         // 펼쳐지는 애니메이션 시간

    private List<BaitSlotUI> spawnedBaitSlots = new List<BaitSlotUI>();
    private List<ItemBaitData> availableBaits = new List<ItemBaitData>();
    private int selectedBaitIndex = 0;
    private bool isOpen = false;

    // 미끼 슬롯 내부 데이터 저장을 위한 구조체/클래스
    private class BaitSlotUI
    {
        public GameObject rootObject;
        public RectTransform rectTransform;
        public Image iconImage;
        public ItemBaitData baitData;
        public Vector2 targetAnchoredPosition; // 최종 목표 위치
    }

    private void Start()
    {
        if (baitSelectorRoot != null)
        {
            baitSelectorRoot.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 1. 도구가 장착된 상태에서 Ctrl 키를 누르고 있을 때
        bool isToolEquipped = CheckToolEquipped();

        if (isToolEquipped && Input.GetKeyDown(KeyCode.LeftControl))
        {
            OpenBaitSelector();
        }
        else if (isToolEquipped && Input.GetKeyUp(KeyCode.LeftControl))
        {
            CloseBaitSelector();
        }

        // 2. 미끼 UI가 열려있는 동안 마우스 휠로 선택 변경
        if (isOpen && spawnedBaitSlots.Count > 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                if (scroll > 0f)
                {
                    selectedBaitIndex = (selectedBaitIndex - 1 + spawnedBaitSlots.Count) % spawnedBaitSlots.Count;
                }
                else
                {
                    selectedBaitIndex = (selectedBaitIndex + 1) % spawnedBaitSlots.Count;
                }
                UpdateSelectionIndicator();
            }
        }
    }

    private bool CheckToolEquipped()
    {
        if (PlayerInventoryController.Instance == null) return false;
        ItemData equipped = PlayerInventoryController.Instance.GetCurrentlyEquippedItem();
        return equipped is ItemToolData;
    }

    private void OpenBaitSelector()
    {
        // 현재 장착된 도구 정보 가져오기
        ItemToolData toolData = PlayerInventoryController.Instance.GetCurrentlyEquippedItem() as ItemToolData;
        if (toolData == null) return;

        // 1 & 3번 수정사항 반영: 현재 장착된 도구 타입과 일치하고, 도구 허용 등급 이하인 미끼만 필터링
        RefreshAvailableBaits(toolData);

        if (availableBaits.Count == 0)
        {
            Debug.Log("이 도구와 매칭되는 사용 가능한 미끼가 없습니다.");
            return;
        }

        // 슬롯 UI 생성 및 배치 각도 계산
        CreateBaitSlotsUI();

        // 2번 수정사항 반영: 현재 장착된 미끼가 있다면 그 인덱스를 찾아 포커싱, 없으면 0번
        ItemBaitData currentEquippedBait = PlayerInventoryController.Instance.GetCurrentlyEquippedBait(toolData.ToolType);
        selectedBaitIndex = 0; // 기본값 0번

        if (currentEquippedBait != null)
        {
            int foundIndex = availableBaits.IndexOf(currentEquippedBait);
            if (foundIndex != -1)
            {
                selectedBaitIndex = foundIndex;
            }
        }

        // [핵심 수정] 도구가 바뀌어 사용 가능한 미끼 개수가 줄어들었을 때 OutOfRange가 나지 않도록 안전장치 보정
        if (selectedBaitIndex >= availableBaits.Count)
        {
            selectedBaitIndex = availableBaits.Count - 1;
        }

        isOpen = true;
        baitSelectorRoot.SetActive(true);
        UpdateSelectionIndicator();

        // 펼쳐지는 연출 애니메이션 시작
        StopAllCoroutines();
        StartCoroutine(AnimateOpenSelector());
    }

    private void CloseBaitSelector()
    {
        isOpen = false;
        if (baitSelectorRoot != null)
        {
            baitSelectorRoot.SetActive(false);
        }

        // 선택된 최종 미끼를 플레이어 컨트롤러나 인벤토리에 전달
        ApplySelectedBait();
    }

    private void RefreshAvailableBaits(ItemToolData toolData)
    {
        availableBaits.Clear();

        if (InventoryManager.Instance != null)
        {
            List<ItemSlot> allItems = InventoryManager.Instance.GetSlots(ItemType.Bait);
            foreach (var item in allItems)
            {
                if (item.ItemData is ItemBaitData baitData)
                {
                    // [수정사항 1 & 3] 미끼의 TargetToolType이 현재 도구의 ToolType과 일치하고, 최대 허용 등급 이하인 경우만 추가
                    if (baitData.TargetToolType == toolData.ToolType && baitData.BaitTier <= toolData.MaxAllowedBaitTier)
                    {
                        if (!availableBaits.Contains(baitData))
                        {
                            availableBaits.Add(baitData);
                        }
                    }
                }
            }
        }
    }

    private void CreateBaitSlotsUI()
    {
        // 기존 생성된 슬롯 정리
        foreach (var slot in spawnedBaitSlots)
        {
            if (slot.rootObject != null) Destroy(slot.rootObject);
        }
        spawnedBaitSlots.Clear();

        int count = availableBaits.Count;
        if (count == 0) return;

        float startAngle = -45f;
        float angleStep = count > 1 ? 90f / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            GameObject slotObj = Instantiate(baitSlotUIPrefab, baitSlotsParent);
            RectTransform rect = slotObj.GetComponent<RectTransform>();
            Image iconImg = slotObj.GetComponent<Image>();

            if (iconImg != null && availableBaits[i].Icon != null)
            {
                iconImg.sprite = availableBaits[i].Icon;
            }

            float currentAngle = startAngle + (angleStep * i);
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 targetPos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            BaitSlotUI slotUI = new BaitSlotUI
            {
                rootObject = slotObj,
                rectTransform = rect,
                iconImage = iconImg,
                baitData = availableBaits[i],
                targetAnchoredPosition = targetPos
            };

            // 처음 시작할 때는 플레이어 위치(중앙, 0,0)에서 시작하도록 설정 (펼쳐지는 연출용)
            rect.anchoredPosition = Vector2.zero;
            spawnedBaitSlots.Add(slotUI);
        }

        UpdateSelectionIndicator();
    }

    private IEnumerator AnimateOpenSelector()
    {
        float timer = 0f;

        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / openDuration);
            float curvedProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);

            for (int i = 0; i < spawnedBaitSlots.Count; i++)
            {
                Vector2 currentPos = Vector2.Lerp(Vector2.zero, spawnedBaitSlots[i].targetAnchoredPosition, curvedProgress);
                spawnedBaitSlots[i].rectTransform.anchoredPosition = currentPos;
            }

            // 애니메이션 도중에도 테두리 위치 동기화
            UpdateSelectionIndicator();

            yield return null;
        }

        // 최종 위치 보정
        for (int i = 0; i < spawnedBaitSlots.Count; i++)
        {
            spawnedBaitSlots[i].rectTransform.anchoredPosition = spawnedBaitSlots[i].targetAnchoredPosition;
        }
        UpdateSelectionIndicator();
    }

    private void UpdateSelectionIndicator()
    {
        if (spawnedBaitSlots.Count == 0 || selectionIndicator == null) return;

        // [방어 코드 추가] selectedBaitIndex가 리스트 범위를 벗어났다면 마지막 유효한 인덱스로 보정
        if (selectedBaitIndex >= spawnedBaitSlots.Count)
        {
            selectedBaitIndex = spawnedBaitSlots.Count - 1;
        }
        // 음수가 되는 경우 방지
        if (selectedBaitIndex < 0)
        {
            selectedBaitIndex = 0;
        }

        // 선택된 미끼 슬롯의 위치로 빨간 테두리(Indicator) 이동
        selectionIndicator.position = spawnedBaitSlots[selectedBaitIndex].rectTransform.position;
    }

    private void ApplySelectedBait()
    {
        if (spawnedBaitSlots.Count == 0) return;

        ItemBaitData selectedBait = spawnedBaitSlots[selectedBaitIndex].baitData;
        if (PlayerInventoryController.Instance != null)
        {
            ItemData equipped = PlayerInventoryController.Instance.GetCurrentlyEquippedItem();

            if (equipped is ItemToolData toolData)
            {
                PlayerInventoryController.Instance.EquipBaitForTool(toolData.ToolType, selectedBait);
                Debug.Log($"미끼 선택 완료: {selectedBait.ItemName}");
            }
        }
    }
}