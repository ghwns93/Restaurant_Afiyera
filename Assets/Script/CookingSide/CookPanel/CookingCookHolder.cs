using UnityEngine;
using System.Collections.Generic;

public class CookingCookHolder : MonoBehaviour
{
    [SerializeField] private CookingSlot _slot;
    [SerializeField] private CookingCookTimer _timer;
    [SerializeField] private CookType _cookType;

    [SerializeField] private List<ItemData> _resourceData;

    private int[] _cookStartTime;
    private bool[] _matCookDone;
    private int _currentStep = 0;

    public bool _isCooking = false;
    public bool _isSeasoned = false;

    private void Awake()
    {
        _slot = this.GetComponent<CookingSlot>();
        _cookStartTime = new int[2] { -1, -1 };
        _matCookDone = new bool[2];

        _slot.OnImageDroppedEvent += ((x) => AddResourceAndCheck());
        _timer.OnTimerStep += CheckCookTime;
    }

    private void AddResourceAndCheck()
    {
        GameObject go = this.transform.GetChild(1).gameObject;
        _resourceData.Add(go.GetComponent<CookingRefineResource>()._data);
        _slot.OnImageMoved(null);
        Destroy(go);

        if (_resourceData.Count == 1)
        {
            _isCooking = true;
            _matCookDone[0] = false;
            _matCookDone[1] = false;
            _cookStartTime[0] = 0;
            _cookStartTime[1] = -1;
            _timer.ActiveCookTimer();
        }
        else
        {
            _matCookDone[1] = false;
            _cookStartTime[1] = _currentStep;
            _slot._isSnapped = true;
        }
    }

    private void CheckCookTime(int time)
    {
        _currentStep = time;
        Debug.Log($"time goes {time}");

        int elapsed0 = time - _cookStartTime[0];
        int elapsed1 = (_cookStartTime[1] >= 0 && _resourceData.Count > 1) ? time - _cookStartTime[1] : -1;

        if (elapsed0 > _resourceData[0].cookTime)
        {
            Debug.Log("Mat 1 Burned");
            DishBurned();
            return;
        }

        if (elapsed1 > 0 && elapsed1 > _resourceData[1].cookTime)
        {
            Debug.Log("Mat 2 Burned");
            DishBurned();
            return;
        }

        if (elapsed0 == _resourceData[0].cookTime)
        {
            _matCookDone[0] = true;
            Debug.Log("mat 1 Complete");
        }

        if (elapsed1 > 0 && elapsed1 == _resourceData[1].cookTime)
        {
            _matCookDone[1] = true;
            Debug.Log("mat 2 Complete");
        }

        if (_resourceData.Count > 1 && _matCookDone[0] && _matCookDone[1])
        {
            Debug.Log("All Complete");
            EndCook(false);
            return;
        }

        if (time == 5)
        {
            bool mat1Ready = _matCookDone[0];
            bool mat2Ready = _resourceData.Count < 2 || _matCookDone[1];

            if (!mat1Ready || !mat2Ready)
            {
                Debug.Log("Time Out");
                DishBurned();
            }
        }   
    }

    private void DishBurned()
    {
        Debug.Log("dish is Burned!");
        EndCook(true);
    }

    private void EndCook(bool isBurned)
    {
        GameObject go = Instantiate(CookingCookTypeManager.Instance.FoodPrefab, this.transform);

        if (!isBurned)
            go.GetComponent<CookingFoodResource>().SetFoodData(
                CookingCookTypeManager.Instance.GetFoodDataByMat(_resourceData[0].id, _resourceData[1].id, this._cookType),this._isSeasoned);
        else
            go.GetComponent<CookingFoodResource>().SetFoodData(
                CookingCookTypeManager.Instance.GetFoodDataById(1000000),this._isSeasoned);

        _timer.EndCookTimer();
        _resourceData.Clear();
        _matCookDone[0] = false;
        _matCookDone[1] = false;
        _cookStartTime[0] = -1;
        _cookStartTime[1] = -1;
        _isSeasoned = false;
        _currentStep = 0;
        _isCooking = false;
    }
}