using System;
using UnityEngine;
using UnityEngine.UI;

public class CookingSlot : MonoBehaviour
{
    [SerializeField] DragImageType _type = DragImageType.None;
    public bool _isSnapped = false;
    public event Action<DragImage> OnImageDroppedEvent;
    public event Action<DragImage> OnImageMovedEvent;

    public DragImageType Type { get { return _type; } }

    public void OnImageDropped(DragImage image)
    {
        _isSnapped = true;
        Debug.Log("Image Dropped.");
        OnImageDroppedEvent?.Invoke(image);
    }

    public void OnImageMoved(DragImage image)
    {
        Debug.Log("?");
        _isSnapped=false;
        Debug.Log("Image Moved");
        OnImageMovedEvent?.Invoke(image);
    }
}