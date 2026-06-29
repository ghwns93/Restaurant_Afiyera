using System;
using UnityEngine;
using UnityEngine.UI;

public class CookingSlot : MonoBehaviour
{
    [SerializeField] DragImageType _type = DragImageType.None;
    public event Action OnImageDroppedEvent;

    public DragImageType Type { get { return _type; } }
    public void OnImageDropped(DragImage image)
    {
        Debug.Log("Image Dropped.");
        OnImageDroppedEvent?.Invoke();
    }
}