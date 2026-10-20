using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenUIManager : MonoBehaviour
{
    public static GardenUIManager Instance;

    [Header("Bottom Slot Settings")]
    public Transform slotParentTransform;
    public GameObject slotButtonPrefab;

    [Header("Selection Panel (연보라 창)")]
    public GameObject selectionPanel;
    public Transform contentTransform;
    public GameObject selectionItemButtonPrefab;
    public ScrollRect selectionScrollRect; // 연보라 창에 붙어있는 ScrollRect 연결

    private int currentActiveSlotIndex;
    private GardenSlotUI currentActiveSlotUI;

    private List<GardenSlotUI> spawnedSlots = new List<GardenSlotUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (selectionPanel != null)
            selectionPanel.SetActive(false);
    }

    private void Start()
    {
        CreateBottomSlots();
    }

    // 빨간 포인트 갯수만큼 항상 빈 슬롯 버튼들을 생성하여 화면에 띄워줍니다.
    private void CreateBottomSlots()
    {
        if (GardenManager.Instance == null || GardenManager.Instance.placePoints == null)
            return;

        foreach (Transform child in slotParentTransform)
        {
            Destroy(child.gameObject);
        }
        spawnedSlots.Clear();

        int pointCount = GardenManager.Instance.placePoints.Count;

        for (int i = 0; i < pointCount; i++)
        {
            GameObject slotObj = Instantiate(slotButtonPrefab, slotParentTransform);
            GardenSlotUI slotUI = slotObj.GetComponent<GardenSlotUI>();

            if (slotUI != null)
            {
                slotUI.InitSlot(i);

                // [핵심] 씬이 시작되거나 생성될 때, GardenRewardManager에 이미 저장된 데이터가 있다면 그에 맞춰 UI를 복원합니다.
                GardenObjectData savedObj = null;
                ItemIngredientData savedMat = null;

                if (GardenRewardManager.Instance != null && GardenRewardManager.Instance.slotDataMap.ContainsKey(i))
                {
                    savedObj = GardenRewardManager.Instance.slotDataMap[i].selectedObject;
                    savedMat = GardenRewardManager.Instance.slotDataMap[i].selectedMaterial;
                }

                slotUI.UpdateUI(savedObj, savedMat);
                spawnedSlots.Add(slotUI);
            }
        }
    }

    // 1번 버튼 클릭: 오브젝트 선택 리스트 출력
    public void OpenObjectSelectionPanel(int slotIndex, GardenSlotUI slotUI)
    {
        currentActiveSlotIndex = slotIndex;
        currentActiveSlotUI = slotUI;

        selectionPanel.SetActive(true);

        ClearSelectionContent();

        // 1. 현재 정원에 이미 설치되어 있는 오브젝트들의 개수를 미리 카운트합니다.
        Dictionary<GardenObjectData, int> installedCounts = new Dictionary<GardenObjectData, int>();
        if (GardenRewardManager.Instance != null)
        {
            foreach (var pair in GardenRewardManager.Instance.slotDataMap)
            {
                // 현재 설정하려는 슬롯 자기 자신은 제외하고 카운트해야 중복 계산이 안 됩니다.
                if (pair.Key == slotIndex) continue;

                var placedObj = pair.Value.selectedObject;
                if (placedObj != null)
                {
                    if (!installedCounts.ContainsKey(placedObj))
                        installedCounts[placedObj] = 0;

                    installedCounts[placedObj]++;
                }
            }
        }

        // 2. 인벤토리에서 불러온 가든 아이템들을 순회하며 리스트 버튼 생성
        foreach (var slot in GardenManager.Instance.availableGardenObjects)
        {
            if (slot.ItemData == null) continue;

            GameObject itemObj = Instantiate(selectionItemButtonPrefab, contentTransform);

            var img = itemObj.transform.Find("Icon")?.GetComponent<Image>();
            if (img != null) img.sprite = slot.ItemData.Icon;

            var btn = itemObj.transform.Find("Icon")?.GetComponent<Button>();
            GardenObjectData gardenObj = (GardenObjectData)slot.ItemData;

            // 이미 정원에 설치된 개수 확인 (없으면 0개)
            int currentInstalledCount = installedCounts.ContainsKey(gardenObj) ? installedCounts[gardenObj] : 0;

            // 인벤토리에 있는 총 수량 (ItemSlot의 Quantity)
            int inventoryQuantity = slot.Quantity;

            // [핵심] 설치된 개수가 인벤토리 보유 수량 이상이면 버튼 비활성화
            bool canPlace = currentInstalledCount < inventoryQuantity;

            if (btn != null)
            {
                btn.interactable = canPlace;

                // 선택 가능할 때만 클릭 이벤트 연결
                if (canPlace)
                {
                    GardenObjectData clickedObj = gardenObj;
                    btn.onClick.AddListener(() => {
                        SelectGardenObject(clickedObj);
                    });
                }
            }

            // (선택사항) 수량이 부족할 때 시각적으로 다르게 보이도록 투명도 조절 등을 추가할 수도 있습니다.
            if (img != null && !canPlace)
            {
                Color c = img.color;
                c.a = 0.4f; // 흐리게 표시
                img.color = c;
            }
        }

        ResetScrollPosition();
    }

    // 2번 버튼 클릭: 재료 선택 리스트 출력
    public void OpenMaterialSelectionPanel(int slotIndex, GardenSlotUI slotUI)
    {
        currentActiveSlotIndex = slotIndex;
        currentActiveSlotUI = slotUI;

        // 데이터 체크는 이제 영구 데이터 매니저(GardenRewardManager)를 기준으로 확인합니다.
        if (GardenRewardManager.Instance == null ||
            !GardenRewardManager.Instance.slotDataMap.ContainsKey(slotIndex) ||
            GardenRewardManager.Instance.slotDataMap[slotIndex].selectedObject == null)
        {
            Debug.LogWarning("먼저 오브젝트를 선택해야 해당 재료를 고를 수 있습니다!");
            return;
        }

        selectionPanel.SetActive(true);

        ClearSelectionContent();

        GardenObjectData currentObj = GardenRewardManager.Instance.slotDataMap[slotIndex].selectedObject;
        ResourceType targetType = currentObj.resourceType;

        List<ItemIngredientData> allIngredients = IngredientDicManager.Instance.GetAllDataList();

        foreach (var ingredient in allIngredients)
        {
            if (ingredient.resourceType == targetType && ingredient.isUnlocked)
            {
                GameObject itemObj = Instantiate(selectionItemButtonPrefab, contentTransform);

                var img = itemObj.transform.Find("Icon")?.GetComponent<Image>();
                if (img != null) img.sprite = ingredient.Icon;

                var btn = itemObj.transform.Find("Icon")?.GetComponent<Button>();
                ItemIngredientData clickedIngredient = ingredient;
                btn.onClick.AddListener(() => {
                    SelectMaterial(clickedIngredient);
                });
            }
        }

        ResetScrollPosition();
    }

    // 스크롤을 맨 왼쪽으로 보내는 메서드
    private void ResetScrollPosition()
    {
        if (selectionScrollRect != null)
        {
            // Canvas 레이아웃이 즉시 반영되도록 갱신 후 위치 이동
            Canvas.ForceUpdateCanvases();
            selectionScrollRect.horizontalNormalizedPosition = 0f;
        }
    }

    private void ClearSelectionContent()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectGardenObject(GardenObjectData gardenObj)
    {
        // GardenManager의 변경된 메서드(ChangeGardenObject)를 호출하여 데이터 저장 및 월드 갱신 처리
        GardenManager.Instance.ChangeGardenObject(currentActiveSlotIndex, gardenObj);

        var slotData = GardenRewardManager.Instance.slotDataMap[currentActiveSlotIndex];
        currentActiveSlotUI.UpdateUI(slotData.selectedObject, slotData.selectedMaterial);

        selectionPanel.SetActive(false);
    }

    private void SelectMaterial(ItemIngredientData material)
    {
        // GardenManager의 변경된 메서드(ChangeMaterial)를 호출하여 데이터 저장 처리
        GardenManager.Instance.ChangeMaterial(currentActiveSlotIndex, material);

        var slotData = GardenRewardManager.Instance.slotDataMap[currentActiveSlotIndex];
        currentActiveSlotUI.UpdateUI(slotData.selectedObject, slotData.selectedMaterial);

        selectionPanel.SetActive(false);
    }
}