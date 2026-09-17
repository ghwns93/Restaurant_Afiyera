using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용 중이시라면 유지, 기본 Text라면 UnityEngine.UI로 변경

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image HighlightImage;          // 선택아이템 표시용 하이라이트
    [SerializeField] private Image iconImage;               // 아이템 아이콘을 표시할 이미지 컴포넌트
    [SerializeField] private TextMeshProUGUI quantityText;  // 수량을 표시할 텍스트 컴포넌트 (기본 Text라면 Text로 변경)

    private ItemData currentItemData;
    private int currentQuantity;

    public ItemData CurrentItemData { get => currentItemData; set => currentItemData = value; }
    public int CurrentQuantity { get => currentQuantity; set => currentQuantity = value; }

    // 슬롯에 아이템 정보 반영
    public void SetItem(ItemData itemData, int quantity)
    {
        CurrentItemData = itemData;
        CurrentQuantity = quantity;

        if (iconImage != null)
        {
            iconImage.sprite = itemData.Icon;
            iconImage.gameObject.SetActive(true);
        }

        if (quantityText != null)
        {
            // 수량이 1개 초과일 때만 숫자를 표시하고, 도구처럼 1개인 경우 숨길 수도 있습니다.
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.text = string.Empty;
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    // 슬롯을 비우는 메서드
    public void ClearSlot()
    {
        CurrentItemData = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.gameObject.SetActive(false);
        }
    }

    public void HighlightSlot()
    {
        HighlightImage.color = Color.greenYellow;
    }

    public void ClearHighlight()
    {
        HighlightImage.color = Color.white;
    }
}