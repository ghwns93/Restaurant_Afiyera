using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    [SerializeField] private ItemData itemData;          // 아이템 기본 데이터 (이름, 설명, 아이콘 등 포함)
    [SerializeField] private int basePrice;              // 기본 가격
    [SerializeField] private int currentPrice;           // 현재 가격 (이벤트 적용 후)
    [SerializeField] private int previousPrice;          // 전날 가격 (가격 변동 비교용)

    public ItemData ItemData => itemData;
    public int CurrentPrice => currentPrice;
    public int PreviousPrice => previousPrice;

    // 전날 대비 가격 증감량 계산 (양수면 상승, 음수면 하락)
    public int PriceChangeAmount => currentPrice - previousPrice;

    public void InitializePrice()
    {
        currentPrice = basePrice; // 초기 가격은 기본 가격으로 설정
        previousPrice = basePrice; // 전날 가격도 초기에는 기본 가격으로 설정
    }

    public void InitializePrice(int current, int previous)
    {
        currentPrice = current; // 초기 가격은 기본 가격으로 설정
        previousPrice = previous; // 전날 가격도 초기에는 기본 가격으로 설정
    }

    // 가격 변경 메서드 (이벤트 발생 시 호출)
    public void UpdatePrice(int newPrice)
    {
        previousPrice = currentPrice; // 현재 가격을 전날 가격으로 백업
        currentPrice = newPrice;      // 새로운 가격 설정

        Debug.Log($"[ShopItemData] Price updated for {itemData.ItemName}: Previous Price = {previousPrice}, Current Price = {currentPrice}");
    }
}