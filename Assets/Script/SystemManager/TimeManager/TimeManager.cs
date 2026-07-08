using System.Collections.Generic;
using UnityEngine;

// 일정시간이 + 되는 방식
public class TimeManager : TimeBase
{
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
        nowMinutes = Mathf.Min((sleepTime * secondsPerMinute), nowMinutes + minutes);
        Debug.Log($"현재 시간 {nowMinutes / secondsPerMinute} 시 {nowMinutes % secondsPerMinute} 분");

        if (nowMinutes >= (workTime * secondsPerMinute) && isWorking == false)
        {
            GoToWork();
        }
        else if (nowMinutes >= (nightOpenTime * secondsPerMinute) 
              && todayNightRestaurantHasOpen == true 
              && todayNightRestaurantIsWorked == false)
        {
            GoToNightWork();
        }
        else if ((sleepTime * secondsPerMinute) <= nowMinutes)
        {
            //강제 취침 이벤트 발생
            GoToSleep();
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

        nowMinutes = 0; // 하루가 끝나면 시간 초기화

        TimeEvents.OnDayEnded?.Invoke(); // 하루 종료 이벤트 발생
    }

    public override void SetNowTime(int hour)
    {
        isWorking = true;
        nowMinutes = hour * secondsPerMinute;
    }

    
}