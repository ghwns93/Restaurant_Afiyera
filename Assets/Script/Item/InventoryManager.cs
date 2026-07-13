using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemSlot> slots = new List<ItemSlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(ItemData item, int count)
    {
        if (item == null || count <= 0) return;

        // 기존에 인벤토리에 있는지 확인
        ItemSlot existingSlot = slots.Find(s => s.itemData.id == item.id);

        if (existingSlot != null)
        {
            // 이미 존재하면 수량만 증가
            existingSlot.quantity += count;
        }
        else
        {
            // 새로 들어온 아이템이면 새 슬롯 생성 후 리스트에 추가
            slots.Add(new ItemSlot(item, count));
        }

        //Debug.Log($"{item.itemName} x{count} 인벤토리 추가 완료.");
    }

    public void ReduceItem(ItemData item, int count)
    {
        if (item == null || count <= 0) return;
        ItemSlot existingSlot = slots.Find(s => s.itemData.id == item.id);

        if (existingSlot != null && existingSlot.quantity > 0)
        {
            existingSlot.quantity -= count;
        }
        else
        {
            return;
        }

        //Debug.Log($"{item.itemName} x {count} 인벤토리 감소 완료.");
    }

    public int GetItem(ItemData item)
    {
        if (item == null) return 0;

        ItemSlot existingSlot = slots.Find(s => s.itemData.id == item.id);

        if (existingSlot != null)
        {
            return existingSlot.quantity;
        }
        else
        {
            return 0;
        }
    }
}
