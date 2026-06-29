using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CookingCookTimer : MonoBehaviour
{
    [SerializeField] private GameObject _timer;
    [SerializeField] private Image _fill;
    [SerializeField] private float _clockTime;

    public event Action OnTimerEnd;

    public void ActiveCookTimer()
    {
        _fill.fillAmount = 1;
        //_timer.SetActive(true);
        StartCoroutine(CookTimer());
    }

    IEnumerator CookTimer()
    {
        float nt = _clockTime;

        if(nt<= 0)
        {
            _fill.fillAmount = 1;
            //_timer.SetActive(false);
            OnTimerEnd?.Invoke();
            OnTimerEnd = null;
            yield break;
        }

        while(nt > 0)
        {
            _fill.fillAmount = nt / _clockTime;
            nt -= Time.deltaTime;

            yield return null;
        }

        _fill.fillAmount = 1;
        //_timer.SetActive(false);
        OnTimerEnd?.Invoke();
        OnTimerEnd = null;
    }
}
