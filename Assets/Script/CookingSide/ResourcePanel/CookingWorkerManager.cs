using UnityEngine;

public class CookingWorkerManager : MonoBehaviour
{
    public static CookingWorkerManager Instance { get; private set; }
    [SerializeField] private CookingWorker[] _workers;

    [SerializeField] GameObject _refineObject;
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

    public void CreateRefineObject(ItemData data,Transform pos)
    {
        CookingRefineResource refine = Instantiate(_refineObject, pos).GetComponent<CookingRefineResource>();
        refine._data = data;
        refine._sprite.sprite = data.refineIcon;
    }
}
