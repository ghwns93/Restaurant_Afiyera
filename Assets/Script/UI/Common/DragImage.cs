using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private DragImageType type;

    private RectTransform _rect;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private Vector2 _originPos;
    private Transform _originParent;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if(_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originPos = _rect.anchoredPosition;
        _originParent = transform.parent;

        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null) return;

        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        CookingSlot slot = FindSlot(eventData);

        if(slot != null)
        {
            SnapToSlot(slot);
        }
        else
        {
            _rect.anchoredPosition = _originPos;
        }
    }

    private CookingSlot FindSlot(PointerEventData eventData)
    {
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach(var result in raycastResults)
        {
            CookingSlot slot = result.gameObject.GetComponent<CookingSlot>();
            if (slot != null && slot.Type == type)
                return slot;
        }
        return null;
    }

    private void SnapToSlot(CookingSlot slot)
    {
        transform.SetParent(slot.transform);
        _rect.anchoredPosition = Vector2.zero;

        slot.OnImageDropped(this);
    }
}
public enum DragImageType { None,Refine };
