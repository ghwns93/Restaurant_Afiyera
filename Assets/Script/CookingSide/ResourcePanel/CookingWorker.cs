using UnityEngine;

public class CookingWorker : MonoBehaviour
{
    [SerializeField] private ItemData _data;
    [SerializeField] private CookingTimeClock _clock;
    [SerializeField] private GameObject _matSlot;

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
        CookingWorkerManager.Instance.CreateRefineObject(_data,_matSlot.transform);
    }
}
