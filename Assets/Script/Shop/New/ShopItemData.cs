using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    [SerializeField] private ItemData itemData;          // 아이템 기본 데이터 (이름, 설명, 아이콘 등 포함)
    [SerializeField] private int maxStockCount;          // 최대 재고 수량
    [SerializeField] private bool stockLimited;          // 재고 제한 여부 (true면 재고 제한, false면 무제한)
    [SerializeField] private int basePrice;              // 기본 가격
    [SerializeField] private int currentPrice;           // 현재 가격 (이벤트 적용 후)
    [SerializeField] private int previousPrice;          // 전날 가격 (가격 변동 비교용)

    [SerializeField] private int stockCount;             // 현재 재고 수량

    public ItemData ItemData => itemData;
    public int CurrentPrice => currentPrice;
    public int PreviousPrice => previousPrice;
    public int BasePrice => basePrice;
    public int MaxStockCount => maxStockCount;
    public bool StockLimited => stockLimited;

    // 전날 대비 가격 증감량 계산 (양수면 상승, 음수면 하락)
    public int PriceChangeAmount => currentPrice - previousPrice;

    public int StockCount { get => stockCount; set => stockCount = value; }

    public void InitializePrice()
    {
        currentPrice = BasePrice; // 초기 가격은 기본 가격으로 설정
        previousPrice = BasePrice; // 전날 가격도 초기에는 기본 가격으로 설정
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