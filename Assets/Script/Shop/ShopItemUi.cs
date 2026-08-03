using UnityEngine;
using UnityEngine.UI;

public class ShopItemUi : MonoBehaviour
{
    [SerializeField] private ShopUnlockableItem targetItem; // 해금할 대상 컴포넌트
    [SerializeField] private int price = 100;
    [SerializeField] private Button buyButton;
    [SerializeField] private bool isUnlimitedPurchase = false; // 무제한 구매 가능 여부
    [SerializeField] private ConsumableType consumableType = ConsumableType.Build; // 소모품 타입

    public ShopUnlockableItem TargetItem { get => targetItem; set => targetItem = value; }
    public ConsumableType ConsumableType { get => consumableType; set => consumableType = value; }
    public bool IsUnlimitedPurchase { get => isUnlimitedPurchase; set => isUnlimitedPurchase = value; }

    private void Start()
    {
        //buyButton.onClick.AddListener(OnBuyButtonClicked);
        UpdateBuyButton();
    }

    public void OnBuyButtonClicked()
    {
        // 1. 재화가 충분한지 확인 (예: CurrencyManager.Instance.HasEnoughCoins(price))
        bool hasEnoughMoney = true; // 예시용 true
            Debug.Log($"[구매 완료] {TargetItem.UnlockID} 구매됨. 가격: {price}");

        if (hasEnoughMoney)
        {
            // 2. 재화 차감
            // CurrencyManager.Instance.UseCoins(price);


            if (!IsUnlimitedPurchase)
            {
                // 3. 해금 처리!
                ShopUnlockManager.Instance.Unlock(TargetItem.UnlockID);

                UpdateBuyButton();
            }
            else
            {
                ShopPurchaseHandler.Instance.PurchaseConsumable(TargetItem.UnlockID, ConsumableType, 1);
            }
        }
    }

    private void UpdateBuyButton()
    {
        // 이미 해금되었다면 구매 버튼 비활성화
        bool isUnlocked = ShopUnlockManager.Instance.IsUnlocked(TargetItem.UnlockID);

        buyButton.interactable = !isUnlocked;
    }
}
