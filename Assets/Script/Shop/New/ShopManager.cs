using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager Instance => instance;

    [SerializeField] private GameObject shopWindowPanel;       // 상점 UI 패널 전체
    [SerializeField] private Transform shopContentParent;      // 1. 상점 아이템들이 생성될 Content (Vertical Layout Group 등)
    [SerializeField] private GameObject shopSlotPrefab;        // 생성할 상점 슬롯 프리팹
    [SerializeField] private TextMeshProUGUI playerGoldText;   // 3. 내 골드량 표시 텍스트
    [SerializeField] private Image shopNpcImage;   // npc이미지

    [SerializeField] private List<ShopItemData> shopItemList = new List<ShopItemData>(); // 상점에서 판매하는 아이템 리스트
    private List<ShopSlotUI> spawnedSlots = new List<ShopSlotUI>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        if (shopWindowPanel != null) shopWindowPanel.SetActive(false);
    }

    // 상점 열기
    public void OpenShop(List<ShopItemData> itemList, Sprite npcImage)
    {
        if (itemList != null) shopItemList = itemList;
        else
        {
            CloseShop();
            return;
        }

        if (shopNpcImage != null && npcImage != null) shopNpcImage.sprite = npcImage;
        else
        {
            CloseShop();
            return;
        }

        if (shopWindowPanel != null) shopWindowPanel.SetActive(true);
        RefreshShopUI();
    }

    // 상점 닫기
    public void CloseShop()
    {
        if (shopWindowPanel != null) shopWindowPanel.SetActive(false);

        SystemController.Instance.SetSystemPause(true);
    }

    public void ChangeData(List<ShopItemData> itemList, Sprite npcImage)
    {
        if (itemList != null) shopItemList = itemList;
        else
        {
            CloseShop();
            return;
        }

        if (shopNpcImage != null && npcImage != null) shopNpcImage.sprite = npcImage;
        else
        {
            CloseShop();
            return;
        }

        RefreshShopUI();
    }

    // 1. 상점이 열릴 때 프리팹 안에 해당 정보 넣어주기 (동적 생성)
    private void RefreshShopUI()
    {
        // 기존 생성된 슬롯 청소
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        // 리스트를 순회하며 슬롯 프리팹 생성 및 데이터 주입
        foreach (var shopItem in shopItemList)
        {
            if (shopSlotPrefab != null && shopContentParent != null)
            {
                GameObject slotObj = Instantiate(shopSlotPrefab, shopContentParent);
                ShopSlotUI slotUI = slotObj.GetComponent<ShopSlotUI>();

                if (slotUI != null)
                {
                    slotUI.SetUpSlot(shopItem);
                    spawnedSlots.Add(slotUI);
                }
            }
        }

        UpdatePlayerGoldUI();
    }

    // 3. 내 골드량 UI 갱신
    public void UpdatePlayerGoldUI()
    {
        if (playerGoldText != null && InventoryManager.Instance != null)
        {
            // InventoryManager에 소지 골드를 관리하는 변수가 있다고 가정 (예: PlayerGold)
            playerGoldText.text = InventoryManager.Instance.GetGold().ToString();
        }
    }

    //[추가] 외부 버튼이나 더블클릭 시 호출할 수 있는 public 구매 함수
    public void BuyItem(ShopItemData shopItem)
    {
        if (shopItem == null || shopItem.ItemData == null) return;

        int itemPrice = shopItem.CurrentPrice;

        // 1. 소지 골드 확인 (InventoryManager에 골드 확인 및 차감 기능이 있다고 가정)
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.GetGold() >= itemPrice)
            {
                // 2. 골드 차감 및 인벤토리에 아이템 추가
                InventoryManager.Instance.ReduceGold(itemPrice); // 혹은 골드 감소 메서드명
                InventoryManager.Instance.AddItem(shopItem.ItemData, 1); // 인벤토리에 추가하는 메서드

                // 3. UI 갱신
                UpdatePlayerGoldUI();
                Debug.Log($"[상점] {shopItem.ItemData.ItemName}을(를) {itemPrice} 골드에 구매했습니다!");
            }
            else
            {
                Debug.Log("[상점] 골드가 부족합니다!");
                // TODO: 골드 부족 팝업이나 UI 피드백 연동 가능
            }
        }
    }
}