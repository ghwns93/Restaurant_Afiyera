using System.Collections.Generic;
using UnityEngine;

public class ShopPlayerInventorySetting : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] GameObject slotPrefab;
    private List<ShopUserInvenSlotPrefabScript> inventorySlots = new List<ShopUserInvenSlotPrefabScript>();

    public void InitSlots()
    {
        // 기존 생성된 슬롯 UI 청소
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        inventorySlots.Clear();

        // 인벤토리 매니저의 슬롯 개수(유동적으로 늘어난 크기)만큼 UI 슬롯 생성
        List<ItemSlot> slots = InventoryManager.Instance.GetSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].ItemData != null)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotParent);
                ShopUserInvenSlotPrefabScript slotUI = slotObj.GetComponent<ShopUserInvenSlotPrefabScript>();

                slotUI.SetItem(slots[i], i);

                inventorySlots.Add(slotUI);
            }
        }
    }
}
