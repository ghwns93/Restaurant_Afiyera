using UnityEngine;
using System.Collections.Generic;

public class CookingCookHolder : MonoBehaviour
{
    [SerializeField] private CookingSlot _slot;
    [SerializeField] private CookingCookTimer _timer;

    [SerializeField] private List<ItemData> _resourceData;

    private void Awake()
    {
        _slot = this.GetComponent<CookingSlot>();

        _slot.OnImageDroppedEvent += AddResourceAndCheck;
    }

    private void AddResourceAndCheck()
    {
        if(_resourceData.Count == 0)
        {
            GameObject go = this.transform.GetChild(1).gameObject;
            _resourceData.Add(go.GetComponent<CookingRefineResource>()._data);
            _timer.ActiveCookTimer();
            Destroy(go);
        }
        else
        {
            GameObject go = this.transform.GetChild(1).gameObject;
            _resourceData.Add(go.GetComponent<CookingRefineResource>()._data);
            Destroy(go);
        }
    }
}
