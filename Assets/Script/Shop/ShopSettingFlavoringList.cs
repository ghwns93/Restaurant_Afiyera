using UnityEngine;

public class ShopSettingFlavoringList : MonoBehaviour
{
    [SerializeField]
    private GameObject ShopBtnPrefab;

    [SerializeField]
    private GameObject ShopListPanel;

    private void Start()
    {
        SettingList();
    }

    private void SettingList()
    {
        var shopItems = FlavoringDicManager.Instance.GetTypeValue(ItemType.Flavoring);

        foreach (var item in shopItems)
        {
            var priceInfo = item.GetComponent<ShopUnlockableItem>();

            if (!ShopUnlockManager.Instance.IsUnlocked(priceInfo.UnlockID))
            {
                GameObject newButton = Instantiate(ShopBtnPrefab, ShopListPanel.transform);
                var buttonComponent = newButton.GetComponent<ShopItemUi>();

                if (buttonComponent != null)
                {
                    var NodeInfo = item.GetComponent<FlavoringData>();
                    var buttonInfo = newButton.GetComponent<SetShopButtonInfo>();

                    buttonComponent.TargetItem = priceInfo;
                    buttonComponent.ConsumableType = ConsumableType.Flavoring;
                    buttonComponent.IsUnlimitedPurchase = priceInfo.IsUnlockedByDefault;
                    buttonComponent.Unlockprice = priceInfo.UnlockCost;
                    buttonComponent.Sellprice = priceInfo.SellCost;

                    buttonInfo.SetButton(NodeInfo.ItemData.itemName, priceInfo.UnlockCost.ToString(), NodeInfo.ItemData.icon);

                    if(priceInfo.IsUnlockedByDefault)
                    {
                        buttonComponent.UpdateBuyButton();
                    }
                }
                else
                {
                    Debug.LogWarning("ShopBtnPrefab에 ShopItemUi 컴포넌트가 없습니다.");
                }
            }
        }
    }
}
