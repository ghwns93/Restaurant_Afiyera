using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DayManager : TimeBase
{
    public static DayManager Instance;

    [SerializeField] private float oneDayDurationInSeconds = 300; // 하루가 몇 초인지 설정 (테스트용)

    private float nowOneDayTime = 0;
    private float timePerOnce;

    private int nowTime = 0;

    private const float realSecondsPerDay = 86400; // 실제 하루의 초 수 (24시간 * 60분 * 60초)
    private const int secondsPerHour = 3600; // 한 시간의 초 수

    private Coroutine ClockCoroutine;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        timePerOnce = realSecondsPerDay / oneDayDurationInSeconds; // 실제 시간 대비 게임 내 시간 비율 계산
    }

    private void OnEnable()
    {
        // 이벤트 구독
        SystemController.OnSystemStateChanged += HandleSystemState;
    }

    private void Start()
    {
        StartTime();
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

    private IEnumerator CountTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1); // 1초마다 체크
            nowOneDayTime += timePerOnce; // 게임 내 시간 증가

            if(realSecondsPerDay <= nowOneDayTime)
            {
                nowOneDayTime = 0; // 하루가 끝나면 시간 초기화
                nowTime = 0;
                NextDay(); // 다음 날로 넘어감
            }

            int currentHour = (int)(nowOneDayTime / secondsPerHour); // 현재 시간 계산

            if(currentHour > nowTime)
            {
                nowTime = currentHour; // 시간 업데이트
                //Debug.Log($"현재 시간: {nowTime}시");
            }
        }
    }
}