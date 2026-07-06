using UnityEngine;

public class CookingWorker : MonoBehaviour
{
    [SerializeField] private ItemData _data;
    [SerializeField] private CookingTimeClock _clock;
    [SerializeField] private CookingSlot _matSlot;

    public bool _isWorking = false;

    public void StartWorking(ItemData data)
    {
        _isWorking = true;
        this._data = data;
        _clock.OnTimerEnd += InstantiateMaterial;
        _clock.ActiveFillTimer();
    }

    public void InstantiateMaterial()
    {
        _matSlot.OnImageMovedEvent += (x) => OnMove();
        CookingWorkerManager.Instance.CreateRefineObject(_data, _matSlot.transform);
        this.GetComponentInChildren<CookingSlot>()._isSnapped = true;
    }

    public void OnMove()
    {
        _isWorking = false;
        _matSlot.OnImageMovedEvent -= (x) => OnMove();
    }

}
