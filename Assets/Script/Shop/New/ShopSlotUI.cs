using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIconImage;              // 5. 물건 이미지
    [SerializeField] private TextMeshProUGUI itemNameText;     // 6. 물건 이름
    [SerializeField] private TextMeshProUGUI itemDescText;     // 6. 물건 간략한 설명
    [SerializeField] private TextMeshProUGUI itemPriceText;    // 4. 물건값
    [SerializeField] private Image itemChangeImage;            // 7. 증감 이미지
    [SerializeField] private TextMeshProUGUI priceChangeText;  // 7. 가격 변동 증감량 표시 (예: "+50" 또는 "-20")

    private ShopItemData boundShopItem;                        // 연결된 상점 아이템 데이터

    private int slotIndex; // 몇 번째 상점 슬롯인지 저장

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f; // 더블클릭 판정 시간 간격

    public void SetUpSlot(ShopItemData shopItem)
    {
        boundShopItem = shopItem;

        if (boundShopItem != null && boundShopItem.ItemData != null)
        {
            ItemData data = boundShopItem.ItemData;

            if (itemIconImage != null) itemIconImage.sprite = data.Icon;
            if (itemNameText != null) itemNameText.text = data.ItemName;
            if (itemDescText != null) itemDescText.text = data.Description;
            if (itemPriceText != null) itemPriceText.text = boundShopItem.CurrentPrice.ToString();

            UpdatePriceChangeUI();
        }
    }

    // 가격 변동 증감량 텍스트 업데이트
    public void UpdatePriceChangeUI()
    {
        if (priceChangeText == null || boundShopItem == null) return;

        int changeAmount = boundShopItem.PriceChangeAmount;

        if (changeAmount > 0)
        {
            priceChangeText.text = $"+{changeAmount}";
            priceChangeText.color = Color.red; // 가격 상승 시 빨간색 (원하는 색상으로 변경 가능)
            itemChangeImage.color = Color.red; // 가격 상승 시 빨간색
        }
        else if (changeAmount < 0)
        {
            priceChangeText.text = changeAmount.ToString();
            priceChangeText.color = Color.blue; // 가격 하락 시 파란색
            itemChangeImage.color = Color.blue; // 가격 하락 시 파란색
        }
        else
        {
            priceChangeText.text = "-";
            priceChangeText.color = Color.gray;
            itemChangeImage.color = Color.gray;
        }
    }

    // 마우스 클릭 시 더블클릭 판정
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            // 더블클릭 감지됨! 구매 함수 호출
            if (ShopManager.Instance != null && boundShopItem != null)
            {
                ShopManager.Instance.BuyItem(boundShopItem);
            }
            lastClickTime = 0f; // 초기화
        }
        else
        {
            lastClickTime = Time.time;
        }
    }
}