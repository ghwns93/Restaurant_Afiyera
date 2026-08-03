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
        var shopItems = ((BuildDicManager)BuildDicManager.Instance).GetTypeValue(new CodyNode().GetType());

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
                    buttonComponent.IsUnlimitedPurchase = true;
                    buttonInfo.SetButton(NodeInfo.NodeName, priceInfo.UnlockCost.ToString(), NodeInfo.NodeMarkSprite);
                }
                else
                {
                    Debug.LogWarning("ShopBtnPrefab에 ShopItemUi 컴포넌트가 없습니다.");
                }
            }
        }
    }
}
