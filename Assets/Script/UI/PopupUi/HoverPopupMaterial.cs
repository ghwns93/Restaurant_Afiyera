using UnityEngine;

public class HoverPopupMaterial : HoverPopupTrigger
{
    protected override void SetItemInfo()
    {
        var homeUi = GetComponent<HomeMaterialUiButton>();
        if (homeUi != null)
        {
            var popupScript = popupPanel.GetComponent<MaterialInfoPanelScript>();

            popupScript.SetMaterialInfo(homeUi.itemInfo);
        }
    }
}
