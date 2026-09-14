using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInfoPanelScript : MonoBehaviour
{
    [SerializeField] private Image matImage;
    [SerializeField] private TextMeshProUGUI matNameText;
    [SerializeField] private TextMeshProUGUI matDescriptionText;
    [SerializeField] private TextMeshProUGUI matSubDescriptionText;

    public void SetMaterialInfo(ItemIngredientData showItem)
    {
        if (matImage != null) matImage.sprite = showItem.Icon;
        if (matNameText != null) matNameText.text = showItem.ItemName;
        if (matDescriptionText != null) matDescriptionText.text = showItem.description;

        if (matSubDescriptionText != null)
        {
            if (showItem.buffEffect != null)
            {
                matSubDescriptionText.text = $"버프 효과: {showItem.buffEffect.description}";
            }
            else
            {
                matSubDescriptionText.text = "";
            }
        }
    }
}
