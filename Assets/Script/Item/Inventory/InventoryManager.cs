using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Instance => instance;

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlotCount = 16; // 설정 값에 따라 늘어나는 인벤토리 칸 수
    public List<ItemSlot> slots = new List<ItemSlot>();

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChangedCallback;

    [Header("Inventory Test Settings")]
    [SerializeField] private bool testMode = false; // 테스트 모드 활성화 여부
    [SerializeField] private List<ItemSlot> testItems; // 테스트용 아이템 리스트

    [SerializeField] private int gold = 0; // 골드 초기값

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeInventory();
    }

    private void Start()
    {
        if(testMode)
        {
            foreach (var item in testItems)
            {
                AddItem(item.testItemData, item.testQuantity);
            }
        }
    }

    private void InitializeInventory()
    {
        slots.Clear();
        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new ItemSlot());
        }
    }

    // 슬롯 크기를 동적으로 변경하고 싶을 때 호출
    public void ResizeInventory(int newSize)
    {
        maxSlotCount = newSize;

        if (slots.Count < maxSlotCount)
        {
            int addCount = maxSlotCount - slots.Count;
            for (int i = 0; i < addCount; i++)
            {
                slots.Add(new ItemSlot());
            }
        }
        else if (slots.Count > maxSlotCount)
        {
            slots.RemoveRange(maxSlotCount, slots.Count - maxSlotCount);
        }

        onInventoryChangedCallback?.Invoke();
    }

    public bool AddItem(ItemData item, int count)
    {
        if (item == null || count <= 0) return false;

        // 1. 중첩 가능한 아이템인 경우 기존에 가진 슬롯에 합치기 시도
        if (item.IsStackable)
        {
            ItemSlot existingSlot = slots.Find(s => !s.IsEmpty && s.ItemData.Id == item.Id);
            if (existingSlot != null)
            {
                existingSlot.Quantity += count;
                onInventoryChangedCallback?.Invoke();
                return true;
            }
        }

        // 2. 빈 슬롯을 찾아 새로 배치
        ItemSlot emptySlot = slots.Find(s => s.IsEmpty);
        if (emptySlot != null)
        {
            emptySlot.ItemData = item;
            emptySlot.Quantity = count;
            onInventoryChangedCallback?.Invoke();
            return true;
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public void ReduceItem(ItemData item, int count)
    {
        if (item == null || count <= 0) return;

        ItemSlot existingSlot = slots.Find(s => !s.IsEmpty && s.ItemData.Id == item.Id);
        if (existingSlot != null)
        {
            existingSlot.Quantity -= count;
            if (existingSlot.Quantity <= 0)
            {
                existingSlot.Clear();
            }
            onInventoryChangedCallback?.Invoke();
        }
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;

        int totalCount = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.ItemData.Id == item.Id)
            {
                totalCount += slot.Quantity;
            }
        }
        return totalCount;
    }

    public List<ItemSlot> GetSlots()
    {
        return slots;
    }

    public int GetGold()
    {
        return gold;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        onInventoryChangedCallback?.Invoke();
    }

    public void ReduceGold(int amount)
    {
        if (amount <= 0) return;
        gold -= amount;
        if (gold < 0) gold = 0;
        onInventoryChangedCallback?.Invoke();
    }
}
