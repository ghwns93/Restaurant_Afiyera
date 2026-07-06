using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CookingSeasoningHolder : MonoBehaviour
{
    [SerializeField] private int _seasoningTier;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        this.GetComponentInChildren<DragImage>().OnEndDragEvent += (x) => SeasoningAdded(x);
        _text.text = $"X {CookingTempSeasoningManager.Instance.GetSeasoningCount(_seasoningTier)}";
    }

    private void SeasoningAdded(PointerEventData eventData)
    {
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            CookingCookHolder cook = result.gameObject.GetComponent<CookingCookHolder>();
            if (cook != null && cook._isCooking && !cook._isSeasoned)
            {
                cook._isSeasoned = true;
                CookingTempSeasoningManager.Instance.DiscountSeasning(this._seasoningTier, 1);
                _text.text = $"X {CookingTempSeasoningManager.Instance.GetSeasoningCount(_seasoningTier)}";
            }
        }
    }
}