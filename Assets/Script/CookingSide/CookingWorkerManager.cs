using UnityEngine;

public class CookingWorkerManager : MonoBehaviour
{
    public static CookingWorkerManager Instance { get; private set; }
    [SerializeField] private CookingWorker[] _workers;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public bool CheckIsWorkerCanWork()
    {
        foreach (CookingWorker worker in _workers)
            if (worker._isWorking) return false;

        return true;
    }

    public void StartWorking(ItemData data)
    {
        for(int i = 0; i< _workers.Length; i++)
        {
            if(!_workers[i]._isWorking)
            {   
                _workers[i].StartWorking(data);
                return;
            }
        }
    }
}
