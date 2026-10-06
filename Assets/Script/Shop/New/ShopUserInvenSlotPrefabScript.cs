using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUserInvenSlotPrefabScript : MonoBehaviour
{
    [Header("아이템 정보")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemPrice;

    private int sellPrice = 0;
    private int inventoryIndex = -1;
    private ItemSlot slot;

    public int SellPrice => sellPrice;
    public int InventoryIndex => inventoryIndex;

    public void SetItem(ItemSlot itemSlot, int InventoryIndex)
    {
        slot = itemSlot;
        inventoryIndex = InventoryIndex;

        itemImage.gameObject.SetActive(true);
        itemImage.sprite = slot.ItemData.Icon;

        if (slot.ItemData is not ItemIngredientData)
        {
            sellPrice = slot.ItemData.BaseSellPrice;
        }
        else
        {
            sellPrice = ShopManager.Instance.CheckIngredientPrice((ItemIngredientData)slot.ItemData);
        }

        itemPrice.text = sellPrice.ToString("N0");
    }
}
