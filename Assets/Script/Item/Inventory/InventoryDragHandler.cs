using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image itemIconImage; // 슬롯의 아이템 아이콘 이미지 (인스펙터 연결 혹은 코드에서 Get)

    private static GameObject ghostIconObj;
    private static Image ghostImageIcon;
    [SerializeField] private Canvas mainCanvas;

    private void Awake()
    {
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }

        if (itemIconImage == null)
        {
            itemIconImage = transform.Find("ItemImage")?.GetComponent<Image>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 아이콘이 없거나 아이템이 비어있으면 드래그 불가
        if (itemIconImage == null || itemIconImage.sprite == null) return;

        InventorySlotUI slotUI = GetComponent<InventorySlotUI>();
        int slotIndex = transform.GetSiblingIndex();

        // 인벤토리에 실제 아이템 데이터가 있는지 확인
        if (InventoryManager.Instance != null)
        {
            var slotData = InventoryManager.Instance.slots[slotIndex];
            if (slotData == null || slotData.ItemData == null) return;
        }

        // 고스트 아이콘 생성
        CreateGhostIcon(itemIconImage.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIconObj != null && mainCanvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                eventData.position,
                mainCanvas.worldCamera,
                out Vector2 localPoint
            );
            ghostIconObj.transform.position = mainCanvas.transform.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyGhostIcon();
    }

    private void CreateGhostIcon(Sprite iconSprite)
    {
        DestroyGhostIcon();

        ghostIconObj = new GameObject("DragGhostIcon");
        ghostIconObj.transform.SetParent(mainCanvas.transform, false);
        ghostIconObj.transform.SetAsLastSibling();

        RectTransform rectTransform = ghostIconObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50, 50); // 아이콘 크기

        ghostImageIcon = ghostIconObj.AddComponent<Image>();
        ghostImageIcon.sprite = iconSprite;
        ghostImageIcon.raycastTarget = false; // 마우스 이벤트 방해 금지

        Color color = ghostImageIcon.color;
        color.a = 0.8f;
        ghostImageIcon.color = color;
    }

    private void DestroyGhostIcon()
    {
        if (ghostIconObj != null)
        {
            Destroy(ghostIconObj);
            ghostIconObj = null;
        }
    }
}