using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DragImageType type;

    private RectTransform _rect;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private Vector2 _originPos;
    private Transform _originParent;
    private CookingSlot _originSlot;

    public event Action OnBeginDragEvent;
    public event Action<PointerEventData> OnEndDragEvent;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originPos = _rect.anchoredPosition;
        _originParent = transform.parent;
        _originSlot = _originParent.GetComponent<CookingSlot>();
        //if (_originParent.GetComponent<CookingSlot>() != null)
        //    _originParent.GetComponent<CookingSlot>()._isSnapped = false;

        _canvasGroup.blocksRaycasts = false;
        OnBeginDragEvent?.Invoke();

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

        if (slot != null && !slot._isSnapped)
        {
            SnapToSlot(slot);
        }
        else
        {
            _rect.anchoredPosition = _originPos;
            transform.SetParent(_originParent);
            OnEndDragEvent?.Invoke(eventData);
            //_originParent.GetComponent<CookingSlot>()._isSnapped = true;
        }
    }

    private CookingSlot FindSlot(PointerEventData eventData)
    {
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
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
        slot.OnImageDropped(this);
        _rect.anchoredPosition = Vector2.zero;

        if (_originSlot != null)
            _originSlot.OnImageMoved(this);
    }

    public void ResetEvent()
    {
        OnBeginDragEvent = null;
        OnEndDragEvent = null;
    }
}
public enum DragImageType { None, Refine, Food, DishFood, Seasoning };
