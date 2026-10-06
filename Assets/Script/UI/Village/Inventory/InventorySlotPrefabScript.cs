using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotPrefabScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("아이템 정보")]
    [SerializeField] Image itemImage;

    private ItemSlot slot;

    public void SetItem(ItemSlot itemSlot)
    {
        slot = itemSlot;

        itemImage.gameObject.SetActive(true);
        itemImage.sprite = slot.ItemData.Icon;
    }

    public void ClearSlot()
    {
        slot = null;

        itemImage.gameObject.SetActive(false);
    }

    // Update() 문을 아예 제거하여 매 프레임 위치를 갱신하며 발생하는 점멸 원인을 원천 차단합니다.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot == null) return;

        if (InventorySceneManager.Instance != null)
        {
            InventorySceneManager.Instance.ShowTooltip(slot.ItemData.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slot == null) return;

        if (InventorySceneManager.Instance != null)
        {
            InventorySceneManager.Instance.HideTooltip();
        }
    }
}
