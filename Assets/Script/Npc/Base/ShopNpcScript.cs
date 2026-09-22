using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopNpcScript : BasicNpcScript
{
    [SerializeField] private List<ShopItemData> shopItemList = new List<ShopItemData>(); // 상점에서 판매하는 아이템 리스트

    [SerializeField] private List<BuyIngredientData> ingredientItemList = new List<BuyIngredientData>(); // 상점에서 구매하는 아이템 리스트

    public List<ShopItemData> ShopItemList { get => shopItemList; set => shopItemList = value; }
    public List<BuyIngredientData> IngredientItemList { get => ingredientItemList; set => ingredientItemList = value; }

    private void Start()
    {
        StartRoutine();
    }

    public override void StartRoutine()
    {
        base.StartRoutine();
        SetBuyIngredientList();

        // 코루틴을 시작하여 매니저가 생길 때까지 대기
        StartCoroutine(InitializeRoutineAsync());
    }

    private void SetBuyIngredientList()
    {
        var allList = IngredientDicManager.Instance.GetAllDataList();

        foreach (var item in allList)
        {
            BuyIngredientData newData = new BuyIngredientData(item, item.BaseBuyPrice);

            IngredientItemList.Add(newData);
        }
    }

    private IEnumerator InitializeRoutineAsync()
    {
        // NpcLoadManager.Instance가 null이 아닐 때까지 프레임 단위로 대기
        yield return new WaitUntil(() => NpcLoadManager.Instance != null);
        yield return new WaitUntil(() => NpcLoadManager.Instance.loaded);

        var temp = NpcLoadManager.Instance.GetNpcDataById(MyNpcId.ToString());

        if (temp == null)
        {
            NpcData nData = new NpcData
            {
                npcId = MyNpcId.ToString(),
                shopItemList = ShopItemList,
            };

            NpcLoadManager.Instance.NewDataStructure(nData);
        }

        // 1. 상점 아이템 리스트 초기화
        foreach (var item in shopItemList)
        {
            if (item != null)
            {
                if (temp != null)
                {
                    var savedItem = temp.shopItemList.Find(shopitem => shopitem.ItemData.Id == item.ItemData.Id);

                    // 저장된 아이템 데이터가 존재할 때만 이전 가격 반영 (안전 장치)
                    if (savedItem != null)
                    {
                        item.InitializePrice(savedItem.CurrentPrice, savedItem.PreviousPrice);
                    }
                    else
                    {
                        item.InitializePrice();
                        item.StockCount = item.MaxStockCount; // 재고 초기화
                    }
                }
                else
                {
                    item.InitializePrice(); // 가격 초기화
                    item.StockCount = item.MaxStockCount; // 재고 초기화
                }
            }
        }

        if (TimeBase.Instance != null)
        {
            if (TimeBase.Instance.IsNewDay)
            {
                NextDayRandomChangePriceEventHandler(false);
            }
        }
    }

    private void OnEnable()
    {
        // 3. 특정 이벤트 발동 시 가격 변동 리스너 등록 (예시: 날짜 변경 이벤트나 경제 이벤트)
        TimeEvents.OnDayEnded += NextDayRandomChangePriceEventHandler;
    }

    private void OnDisable()
    {
        TimeEvents.OnDayEnded -= NextDayRandomChangePriceEventHandler;
    }

    private void NextDayRandomChangePriceEventHandler(bool nothing)
    {
        float priceMultiplier = Random.Range(0.8f, 1.2f); // 예시: 0.8~1.2 사이의 랜덤 배율
        HandleEconomyEvent(priceMultiplier);

        ChangeBuyPrice();
        StockReplenishment();
    }

    public void HandleEconomyEvent(float priceMultiplier)
    {
        foreach (var shopItem in ShopItemList)
        {
            // 예시: 기존 가격에 배율을 곱해 가격 변동 발생
            int newPrice = Mathf.RoundToInt(shopItem.BasePrice * priceMultiplier);
            shopItem.UpdatePrice(newPrice);
        }

        ShopManager.Instance.ChangeData(ShopItemList, npcImage);

        NpcData nData = new NpcData
        {
            npcId = MyNpcId.ToString(),
            shopItemList = ShopItemList,
        };

        NpcLoadManager.Instance.NewDataStructure(nData);
    }

    private void ChangeBuyPrice()
    {
        foreach (var ingredient in IngredientItemList)
        {
            float priceMultiplier = Random.Range(ingredient.itemData.minPriceMultiplier, ingredient.itemData.maxPriceMultiplier); // 예시: 0.8~1.2 사이의 랜덤 배율
            int newPrice = Mathf.RoundToInt(ingredient.itemData.BaseBuyPrice * priceMultiplier);
            ingredient.buyPrice = newPrice;

            //Debug.Log($"[ShopNpcScript] {ingredient.itemData.ItemName}의 구매 가격이 {ingredient.buyPrice}로 변경되었습니다.");
        }
    }

    private void StockReplenishment()
    {
        foreach (var shopItem in ShopItemList)
        {
            if (!shopItem.StockLimited)
            {
                // 재고 제한이 있는 아이템만 재고 보충
                shopItem.StockCount = shopItem.MaxStockCount; // 최대 재고로 보충
            }
        }
    }
}

public class BuyIngredientData
{
    public ItemIngredientData itemData;
    public int buyPrice;
    public BuyIngredientData(ItemIngredientData itemData, int buyPrice)
    {
        this.itemData = itemData;
        this.buyPrice = buyPrice;
    }
}