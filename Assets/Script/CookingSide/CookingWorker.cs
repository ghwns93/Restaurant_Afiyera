using UnityEngine;

public class CookingWorker : MonoBehaviour
{
    [SerializeField] private int _matID;
    [SerializeField] private CookingTimeClock _clock;
    [SerializeField] private GameObject _matSlot;

    public void StartWorking(int matid)
    {
        this._matID = matid;
        _clock.OnTimerEnd += InstantiateMaterial;
        _clock.ActiveFillTimer();
    }

    public void InstantiateMaterial()
    {

    }
}
