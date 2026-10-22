using System.Collections.Generic;
using UnityEngine;

public class GardenRewardManager : MonoBehaviour
{
    public static GardenRewardManager Instance;

    // 슬롯별 저장 데이터 구조 (정원 씬이 아니어도 항상 유지됨)
    [System.Serializable]
    public class SlotSaveData
    {
        public GardenObjectData selectedObject;
        public ItemIngredientData selectedMaterial;
    }

    // 슬롯 인덱스별 데이터 저장 딕셔너리
    public Dictionary<int, SlotSaveData> slotDataMap = new Dictionary<int, SlotSaveData>();

    private void OnEnable()
    {
        TimeEvents.OnDayEnded += OnNewDayPassed;
    }

    private void OnDisable()
    {
        TimeEvents.OnDayEnded -= OnNewDayPassed;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 특정 슬롯의 오브젝트 데이터 변경 및 저장
    public void SetGardenObject(int slotIndex, GardenObjectData objData)
    {
        if (!slotDataMap.ContainsKey(slotIndex))
            slotDataMap[slotIndex] = new SlotSaveData();

        // 오브젝트가 바뀌었는데 기존 재료의 타입이 안 맞으면 재료 초기화
        if (slotDataMap[slotIndex].selectedObject != objData)
        {
            slotDataMap[slotIndex].selectedMaterial = null;
        }

        slotDataMap[slotIndex].selectedObject = objData;
    }

    // 특정 슬롯의 재료 데이터 변경 및 저장
    public void SetMaterial(int slotIndex, ItemIngredientData material)
    {
        if (!slotDataMap.ContainsKey(slotIndex))
            slotDataMap[slotIndex] = new SlotSaveData();

        slotDataMap[slotIndex].selectedMaterial = material;
        Debug.Log($"[데이터 저장 - 슬롯 {slotIndex}] 재료 설정 완료: {material.ItemName}");
    }

    /// <summary>
    /// 다른 씬(메인, 필드 등)에서 하루가 지났을 때 호출하는 보상 지급 함수
    /// </summary>
    public void OnNewDayPassed(bool none)
    {
        foreach (var pair in slotDataMap)
        {
            SlotSaveData slotData = pair.Value;

            // 석상과 재료가 모두 세팅되어 있는 경우에만 수량 추가
            if (slotData.selectedObject != null && slotData.selectedMaterial != null)
            {
                AddItemToInventory(slotData.selectedMaterial, 1);
                Debug.Log($"[하루 지남 - 보상 지급] 슬롯 {pair.Key}: {slotData.selectedMaterial.ItemName} 1개가 인벤토리에 추가되었습니다.");
            }
        }
    }

    private void AddItemToInventory(ItemIngredientData ingredient, int amount)
    {
        // 사용 중이신 인벤토리 시스템과 연동
        InventoryManager.Instance.AddItem(ingredient, amount);
    }
}