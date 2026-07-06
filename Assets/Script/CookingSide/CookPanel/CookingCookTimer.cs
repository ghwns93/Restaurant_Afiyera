using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CookingCookTimer : MonoBehaviour
{
    [SerializeField] private GameObject _timer;
    [SerializeField] private Image _fill;
    [SerializeField] private float _clockTime;

    private const int STEP_COUNT = 5;

    private Coroutine _coroutine;
    private bool _isCancelled;

    public event Action OnTimerEnd;

    public event Action<int> OnTimerStep;

    public void ActiveCookTimer()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _isCancelled = false;
        _fill.fillAmount = 1;
        //_timer.SetActive(true);
        _coroutine = StartCoroutine(CookTimer());
    }

    public void EndCookTimer()
    {
        if (_coroutine == null) return;

        _isCancelled = true;
        StopCoroutine(_coroutine);
        _coroutine = null;

        ResetTimer();
    }


    private void ResetTimer()
    {
        _fill.fillAmount = 1;
        //_timer.SetActive(false);
        OnTimerEnd = null;
        OnTimerStep = null;
    }
    IEnumerator CookTimer()
    {
        float nt = _clockTime;
        int currentStep = 0;

        if (nt <= 0)
        {
            _fill.fillAmount = 0;
            //_timer.SetActive(false);
            for (int step = currentStep + 1; step <= STEP_COUNT; step++)
                OnTimerStep?.Invoke(step);

            OnTimerEnd?.Invoke();
            OnTimerEnd = null;
            OnTimerStep = null;
            yield break;
        }

        while (nt > 0)
        {
            _fill.fillAmount = nt / _clockTime;
            float elapsed = _clockTime - nt;

            while (currentStep < STEP_COUNT && elapsed >= _clockTime * (currentStep + 1) / STEP_COUNT)
            {
                currentStep++;
                OnTimerStep?.Invoke(currentStep);
            }

            nt -= Time.deltaTime;

            yield return null;
        }

        _fill.fillAmount = 1;

        while (currentStep < STEP_COUNT)
        {
            currentStep++;
            OnTimerStep?.Invoke(currentStep);
        }

        //_timer.SetActive(false);
        OnTimerEnd?.Invoke();
        OnTimerEnd = null;
        OnTimerStep = null;
    }
}
