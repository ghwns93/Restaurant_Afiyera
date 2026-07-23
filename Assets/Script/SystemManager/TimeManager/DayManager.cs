using NUnit.Framework.Internal.Execution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시간이 흐르는 방식
public class DayManager : TimeBase
{
    [SerializeField] private float oneDayDurationInSeconds = 300; // 하루가 몇 초인지 설정 (테스트용)

    private float nowOneDayTime = 0;
    private float timePerOnce;

    private int nowTime = 0;

    private Coroutine ClockCoroutine;

    protected override void Awake()
    {
        base.Awake();

        timePerOnce = realSecondsPerDay / oneDayDurationInSeconds; // 실제 시간 대비 게임 내 시간 비율 계산
    }

    private void OnEnable()
    {
        // 이벤트 구독
        SystemController.OnSystemStateChanged += HandleSystemState;
    }

    private void Start()
    {
        //StartTime();
        SetDefaultTime();
    }

    private void OnDisable()
    {
        PauseTime();

        // 메모리 누수 방지를 위해 해제 필수!
        SystemController.OnSystemStateChanged -= HandleSystemState;
    }

    private void HandleSystemState(bool isPaused)
    {
        if (isPaused)
        {
            StartTime();
        }
        else
        {
            PauseTime();
        }
    }

    public void PauseTime()
    {
        if (ClockCoroutine != null)
        {
            StopCoroutine(ClockCoroutine);
            ClockCoroutine = null;
        }
    }

    public void StartTime()
    {
        if (ClockCoroutine == null)
        {
            ClockCoroutine = StartCoroutine(CountTime());
        }
    }

    private void SetDefaultTime()
    {
        nowOneDayTime = startTime * secondsPerHour; // 하루가 끝나면 시간 초기화
        nowTime = startTime;
    }

    private IEnumerator CountTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1); // 1초마다 체크
            nowOneDayTime += timePerOnce; // 게임 내 시간 증가

            if(nowOneDayTime >= (workTime * secondsPerHour) && isWorking == false)
            {
                GoToWork();
            }
            else if (nowOneDayTime >= (nightOpenTime * secondsPerHour)
                    && todayNightRestaurantHasOpen == true
                    && todayNightRestaurantIsWorked == false)
            {
                GoToNightWork();
            }
            else if(nowOneDayTime >= (sleepTime * secondsPerHour))
            {
                GoToSleep();
            }

            int currentHour = (int)(nowOneDayTime / secondsPerHour); // 현재 시간 계산

            if(currentHour > nowTime)
            {
                nowTime = currentHour; // 시간 업데이트

                TimeEvents.OnNpcSpecificTimeReached?.Invoke(nowTime); // 이벤트 호출
            }
            Debug.Log($"현재 시간: {nowTime}시");
        }
    }

    public override void GoToWork()
    {
        Debug.Log("일하러 갈 시간");
        isWorking = true;

        SystemController.Instance.SetSystemPause(false);
        SceneController.Instance.LoadSubScene(SceneType.Restaurant);
    }

    public override void GoToNightWork()
    {
        Debug.Log("심야식당 오픈!");
        todayNightRestaurantIsWorked = true;

        SystemController.Instance.SetSystemPause(false);
        SceneController.Instance.LoadSubScene(SceneType.NightRestaurant);
    }

    public override void GoToSleep()
    {
        Debug.Log("자러 갈 시간");

        SetDefaultTime();
        NextDay(); // 다음 날로 넘어감
    }

    public override void SetNowTime(int hour)
    {
        isWorking = true;
        nowOneDayTime = hour * secondsPerHour;
        nowTime = hour;
    }
}