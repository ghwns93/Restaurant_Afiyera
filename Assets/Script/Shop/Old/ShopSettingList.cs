using UnityEngine;

public class ShopSettingList : MonoBehaviour
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
        var shopItems = BuildDicManager.Instance.GetTypeValue(new CodyNode().GetType());

        foreach (var item in shopItems)
        {
            var priceInfo = item.GetComponent<ShopUnlockableItem>();

            if (!ShopUnlockManager.Instance.IsUnlocked(priceInfo.UnlockID))
            {
                GameObject newButton = Instantiate(ShopBtnPrefab, ShopListPanel.transform);
                var buttonComponent = newButton.GetComponent<ShopItemUi>();

                if (buttonComponent != null)
                {
                    var NodeInfo = item.GetComponent<BasicNode>();
                    var buttonInfo = newButton.GetComponent<SetShopButtonInfo>();

                    buttonComponent.TargetItem = priceInfo;
                    buttonComponent.ConsumableType = ConsumableType.Build;
                    buttonComponent.IsUnlimitedPurchase = priceInfo.IsUnlockedByDefault;
                    buttonComponent.Unlockprice = priceInfo.UnlockCost;
                    buttonComponent.Sellprice = priceInfo.SellCost;

                    buttonInfo.SetButton(NodeInfo.NodeName, priceInfo.UnlockCost.ToString(), NodeInfo.NodeMarkSprite);

                    if (priceInfo.IsUnlockedByDefault)
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
