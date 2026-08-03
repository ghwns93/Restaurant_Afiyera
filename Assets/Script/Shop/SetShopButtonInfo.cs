using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetShopButtonInfo : MonoBehaviour
{
    private string name;
    private string price;
    private Sprite sprite;

    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private Image image;

    public void SetButton(string name, string price, Sprite sprite)
    {
        this.name = name;
        this.price = price;
        this.sprite = sprite;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (nameText != null) nameText.text = name;
        if (priceText != null) priceText.text = price;
        if (image != null) image.sprite = sprite;
    }
}
