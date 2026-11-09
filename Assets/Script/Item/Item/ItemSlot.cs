using System;

[System.Serializable]
public class ItemSlot
{
    private ItemData itemData; // 아이템 원본 데이터 정보
    private int quantity;      // 현재 수량
    private int usedQuantity;   // 사용된 수량 (예: 장착된 아이템 수량)

    public ItemData testItemData;
    public int testQuantity;

    public ItemData ItemData
    {
        get => itemData;
        set => itemData = value;
    }

    public int Quantity
    {
        get => quantity;
        set => quantity = value;
    }

    public int UsedQuantity
    {
        get => usedQuantity;
        set => usedQuantity = value;
    }

    public ItemSlot()
    {
        itemData = null;
        quantity = 0;
        usedQuantity = 0;
    }

    public ItemSlot(ItemData data, int count, int usedCount)
    {
        itemData = data;
        quantity = count;
        usedQuantity = usedCount;
    }

    public bool IsEmpty => itemData == null || quantity <= 0;

    public void Clear()
    {
        itemData = null;
        quantity = 0;
        usedQuantity = 0;
    }
}