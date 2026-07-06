using UnityEngine;

public class CookingPlate : MonoBehaviour
{
    [SerializeField] private PlateType _type;
    [SerializeField] private RectTransform _target;
    [SerializeField] private CookingSlot _slot;

    public PlateType Type { get { return _type; } }

    public void ReadyPlating()
    {
        _target.gameObject.SetActive(true);
        _slot._isSnapped = false;
        _slot.OnImageDroppedEvent += ExcutePlating;
    }

    public void CancelPlating()
    {
        _target.gameObject.SetActive(false);
        _slot._isSnapped = true;
        _slot.OnImageDroppedEvent -= ExcutePlating;
    }

    public void ExcutePlating(DragImage image)
    {
        float ratio = CookingPlatingManager.Instance.GetOverlapRatioIoU(image.GetComponent<RectTransform>(), _target);
        if (ratio > 0.8f) ratio = 1;
        else if (ratio > 0.6f) ratio = 0.8f;
        else ratio = 0.5f;

        image.GetComponent<CookingFoodResource>().SetDishedFoodData(ratio);
    }

}
