using UnityEngine;
using UnityEngine.UI;

public class CookingTrashBin : MonoBehaviour
{
    [SerializeField] CookingSlot _slot;

    private void Awake()
    {
        this._slot.OnImageDroppedEvent += ((x) => RemoveFood(x));
    }

    private void RemoveFood(DragImage image)
    {
        Destroy(image.gameObject);
        _slot._isSnapped = false;
    }
}
