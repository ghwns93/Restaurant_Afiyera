using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventorySceneScript : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] GameObject slotPrefab;
    private List<InventorySlotPrefabScript> inventorySlots =  new List<InventorySlotPrefabScript>();

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

        for (int i = 0; i < InventoryManager.Instance.MaxSlotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            InventorySlotPrefabScript slotUI = slotObj.GetComponent<InventorySlotPrefabScript>();

            if (slots[i].ItemData != null)
            {
                slotUI.SetItem(slots[i]);
            }
            else
            {
                slotUI.ClearSlot();
            }

            inventorySlots.Add(slotUI);
        }
    }
}
