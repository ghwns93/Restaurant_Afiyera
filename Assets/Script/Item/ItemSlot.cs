[System.Serializable]
public class ItemSlot
{
    public ItemData itemData; // 아이템 원본 데이터 정보
    public int quantity;      // 현재 수량

    public ItemSlot(ItemData data, int count)
    {
        itemData = data;
        quantity = count;
    }
}