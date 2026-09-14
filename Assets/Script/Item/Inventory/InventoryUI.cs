using System.Collections.Generic;
using UnityEngine;
using static PlayerInventoryController;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotParent;   // 그리드 레이아웃 그룹이 있는 부모 객체
    [SerializeField] private GameObject slotPrefab;  // 슬롯 UI 프리팹

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateUI;
            InitSlots();
            UpdateUI();
        }

        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback += OnSelectedSlotChanged;
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback -= UpdateUI;
        }

        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback -= OnSelectedSlotChanged;
        }
    }

    private void InitSlots()
    {
        // 기존 생성된 슬롯 UI 청소
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        // 인벤토리 매니저의 슬롯 개수(유동적으로 늘어난 크기)만큼 UI 슬롯 생성
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUIList.Add(slotUI);
        }
    }

    private void UpdateUI()
    {
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots();

        // 만약 유동적으로 인벤토리 칸 수가 변경되었다면 UI를 재구축
        if (slotUIList.Count != slots.Count)
        {
            InitSlots();
        }

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                slotUIList[i].SetItem(slots[i].ItemData, slots[i].Quantity);
            }
            else
            {
                slotUIList[i].ClearSlot();
            }
        }
    }

    private void OnSelectedSlotChanged(int slotIndex)
    {
        foreach (var slotUI in slotUIList)
        {
            slotUI.ClearHighlight(); // 모든 슬롯의 하이라이트 제거
        }

        slotUIList[slotIndex].HighlightSlot();
    }
}