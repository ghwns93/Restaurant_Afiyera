using UnityEngine;
using UnityEngine.UI;

public class CookingFoodResource : MonoBehaviour
{
    [SerializeField] private FoodData _data;
    [SerializeField] private DragImage _drag;

    [SerializeField] private bool _isSeasoned;
    [SerializeField] private float _rate;
    
    private Image _icon;

    private void Awake()
    {
        _icon = this.GetComponent<Image>();
        _drag = this.GetComponent<DragImage>();
    }

    public void SetFoodData(FoodData data, bool isSeasoned)
    {
        this._data = data;
        this._icon.sprite = data.iconCooked;
        this._isSeasoned = isSeasoned;

        CookingPlate plate = CookingPlatingManager.Instance.FindPlate(data.plateType);

        if (plate != null)
        {
            _drag.OnBeginDragEvent += plate.ReadyPlating;
            _drag.OnEndDragEvent += (x) => plate.CancelPlating();
        }
    }

    public void SetDishedFoodData(float rate)
    {
        _drag.type = DragImageType.DishFood;
        this._icon.sprite = _data.iconPlated;
        _drag.ResetEvent();

        this._rate = rate;
    }
}