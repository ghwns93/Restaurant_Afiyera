using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInfoPanelScript : MonoBehaviour
{
    [SerializeField] private Image matImage;
    [SerializeField] private TextMeshProUGUI matNameText;
    [SerializeField] private TextMeshProUGUI matDescriptionText;

    public void SetMaterialInfo(ItemData showItem)
    {
        if (matImage != null) matImage.sprite = showItem.icon;
        if (matNameText != null) matNameText.text = showItem.itemName;
        if (matDescriptionText != null) matDescriptionText.text = showItem.description;
    }
}
