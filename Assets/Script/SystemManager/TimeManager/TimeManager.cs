using System.Collections.Generic;
using UnityEngine;

public class TimeManager : TimeBase
{
    [SerializeField]
    private int currentMinutes = 15 * 60;

    private int nowMinutes = 0;

    private int nowDay = 0;

    private void OnEnable()
    {
        // 글로벌 이벤트를 구독합니다.
        TimeEvents.OnTimeConsumed += HandleTimeConsumed;
        TimeEvents.OnDayEnded += NextDay;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 구독을 해제합니다.
        TimeEvents.OnTimeConsumed -= HandleTimeConsumed;
        TimeEvents.OnDayEnded -= NextDay;
    }

    private void HandleTimeConsumed(int minutes)
    {
        nowMinutes = Mathf.Min(currentMinutes, nowMinutes + minutes);
        Debug.Log($"현재 시간 {nowMinutes / 60} 시 {nowMinutes % 60} 분");

        if (currentMinutes <= nowMinutes)
        {
            TimeEvents.OnDayEnded?.Invoke(); // 하루 종료 이벤트 발생
        }
    }
}