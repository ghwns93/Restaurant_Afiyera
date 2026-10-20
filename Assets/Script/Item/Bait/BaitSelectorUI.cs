using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaitSelectorUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private GameObject baitSlotUIPrefab;         // 미끼 아이콘 프리팹
    [SerializeField] private Transform baitSlotsParent;           // 미끼들이 배치될 부모 컨테이너
    [SerializeField] private RectTransform centerPointTransform;  // [빨간 점] 회전의 중심 (도구 UI 정중앙)
    [SerializeField] private RectTransform baitPointTransform;    // [녹색 점 / BaitPoint] 선택된 미끼 고정 포인터

    [Header("회전 궤도 설정")]
    [SerializeField] private float slotAngleSpacing = 45f;        // 슬롯 간 각도 간격

    private List<BaitSlotUI> spawnedBaitSlots = new List<BaitSlotUI>();
    private List<ItemBaitData> availableBaits = new List<ItemBaitData>();
    private int selectedBaitIndex = 0;

    // [핵심 수정] static을 붙여서 오브젝트가 꺼지거나 재조립되어도 마지막 선택 미끼 기억이 안 날아가게 유지
    private static Dictionary<ToolType, ItemBaitData> lastSelectedBaitsPerTool = new Dictionary<ToolType, ItemBaitData>();

    private float currentAngleOffset = 0f;
    private Coroutine rotateCoroutine;

    private class BaitSlotUI
    {
        public GameObject rootObject;
        public RectTransform rectTransform;
        public Image iconImage;
        public ItemBaitData baitData;
    }

    private void OnEnable()
    {
        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback += OnEquippedToolChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback -= OnEquippedToolChanged;
        }
    }

    private void Start()
    {
        SetupBaitSelector();
    }

    private void Update()
    {
        HandleMouseWheelInput();
    }

    private void OnEquippedToolChanged(int slotIndex)
    {
        SetupBaitSelector();
    }

    public void SetupBaitSelector()
    {
        ItemToolData toolData = GetCurrentEquippedTool();
        if (toolData == null)
        {
            ClearBaitUI();
            return;
        }

        RefreshAvailableBaits(toolData);

        if (availableBaits.Count == 0)
        {
            ClearBaitUI();
            return;
        }

        selectedBaitIndex = 0;
        ItemBaitData targetBait = null;

        // 1. 기억된 미끼가 있는지 확인
        if (lastSelectedBaitsPerTool.ContainsKey(toolData.ToolType) && lastSelectedBaitsPerTool[toolData.ToolType] != null)
        {
            targetBait = lastSelectedBaitsPerTool[toolData.ToolType];
        }

        // 2. 기억된 게 없거나 인벤토리에 없다면 기본 장착 미끼 확인
        if (targetBait == null || !availableBaits.Contains(targetBait))
        {
            targetBait = PlayerInventoryController.Instance.GetCurrentlyEquippedBait(toolData.ToolType);
        }

        if (targetBait != null)
        {
            int foundIndex = availableBaits.IndexOf(targetBait);
            if (foundIndex != -1)
            {
                selectedBaitIndex = foundIndex;
            }
        }

        if (selectedBaitIndex >= availableBaits.Count)
        {
            selectedBaitIndex = availableBaits.Count - 1;
        }

        if (centerPointTransform != null && baitPointTransform != null)
        {
            Vector2 dir = baitPointTransform.anchoredPosition - centerPointTransform.anchoredPosition;
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currentAngleOffset = baseAngle - (selectedBaitIndex * slotAngleSpacing);
        }

        CreateBaitSlotsUI();
        UpdateAllSlotsPosition();
        ApplySelectedBait();
    }

    private void HandleMouseWheelInput()
    {
        if (spawnedBaitSlots.Count == 0 || centerPointTransform == null || baitPointTransform == null) return;

        // 혹시 Ctrl을 꼭 누른 상태에서만 휠을 돌리게 하고 싶다면 아래 주석을 해제하세요.
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) return;

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

            ItemToolData toolData = GetCurrentEquippedTool();
            if (toolData != null)
            {
                lastSelectedBaitsPerTool[toolData.ToolType] = spawnedBaitSlots[selectedBaitIndex].baitData;
            }

            Vector2 dir = baitPointTransform.anchoredPosition - centerPointTransform.anchoredPosition;
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float targetOffset = baseAngle - (selectedBaitIndex * slotAngleSpacing);

            StopRotateCoroutine();
            rotateCoroutine = StartCoroutine(AnimateRotateTo(targetOffset));

            ApplySelectedBait();
        }
    }

    private ItemToolData GetCurrentEquippedTool()
    {
        if (PlayerInventoryController.Instance == null) return null;
        ItemData equipped = PlayerInventoryController.Instance.GetCurrentlyEquippedItem();
        return equipped as ItemToolData;
    }

    private void ClearBaitUI()
    {
        foreach (var slot in spawnedBaitSlots)
        {
            if (slot.rootObject != null) Destroy(slot.rootObject);
        }
        spawnedBaitSlots.Clear();
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
        foreach (var slot in spawnedBaitSlots)
        {
            if (slot.rootObject != null) Destroy(slot.rootObject);
        }
        spawnedBaitSlots.Clear();

        int count = availableBaits.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            GameObject slotObj = Instantiate(baitSlotUIPrefab, baitSlotsParent);
            RectTransform rect = slotObj.GetComponent<RectTransform>();
            Image iconImg = slotObj.GetComponent<Image>();

            if (iconImg != null && availableBaits[i].Icon != null)
            {
                iconImg.sprite = availableBaits[i].Icon;
            }

            BaitSlotUI slotUI = new BaitSlotUI
            {
                rootObject = slotObj,
                rectTransform = rect,
                iconImage = iconImg,
                baitData = availableBaits[i]
            };

            spawnedBaitSlots.Add(slotUI);
        }
    }

    private void UpdateAllSlotsPosition()
    {
        int count = spawnedBaitSlots.Count;
        if (count == 0 || centerPointTransform == null || baitPointTransform == null) return;

        Vector2 centerPos = centerPointTransform.anchoredPosition;
        float radius = Vector2.Distance(centerPos, baitPointTransform.anchoredPosition);

        for (int i = 0; i < count; i++)
        {
            float angle = (i * slotAngleSpacing) + currentAngleOffset;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            spawnedBaitSlots[i].rectTransform.anchoredPosition = centerPos + offset;
        }
    }

    private IEnumerator AnimateRotateTo(float targetOffset)
    {
        float startOffset = currentAngleOffset;
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            currentAngleOffset = Mathf.Lerp(startOffset, targetOffset, smoothT);
            UpdateAllSlotsPosition();

            yield return null;
        }

        currentAngleOffset = targetOffset;
        UpdateAllSlotsPosition();
    }

    private void StopRotateCoroutine()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
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
                lastSelectedBaitsPerTool[toolData.ToolType] = selectedBait;
            }
        }
    }
}