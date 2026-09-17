using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopNpcScript : BasicNpcScript
{
    [SerializeField] private List<ShopItemData> shopItemList = new List<ShopItemData>(); // 상점에서 판매하는 아이템 리스트

    public List<ShopItemData> ShopItemList { get => shopItemList; set => shopItemList = value; }

    private void Start()
    {
        StartRoutine();
    }

    public override void StartRoutine()
    {
        base.StartRoutine();

        // 코루틴을 시작하여 매니저가 생길 때까지 대기
        StartCoroutine(InitializeRoutineAsync());
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
                    }
                }
                else
                {
                    item.InitializePrice(); // 가격 초기화
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
        Debug.Log("[ShopNpcScript] 날짜가 바뀌어 가격 변동 이벤트 발생!");

        float priceMultiplier = Random.Range(0.8f, 1.2f); // 예시: 0.8~1.2 사이의 랜덤 배율

        HandleEconomyEvent(priceMultiplier);
    }

    public void HandleEconomyEvent(float priceMultiplier)
    {
        foreach (var shopItem in ShopItemList)
        {
            // 예시: 기존 가격에 배율을 곱해 가격 변동 발생
            int newPrice = Mathf.RoundToInt(shopItem.CurrentPrice * priceMultiplier);
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
}