using UnityEngine;

public class ShopSceneUIButton : MonoBehaviour
{
    public void OnCodyShopButtonClick()
    {
        SceneController.Instance.AddtionUiScene(SceneType.ShopCody);
    }

    public void OnFlavoringShopButtonClick()
    {
        SceneController.Instance.AddtionUiScene(SceneType.ShopFlavoring);
    }
}
