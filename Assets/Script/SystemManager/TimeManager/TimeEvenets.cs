using System;

public static class TimeEvents
{
    // 누군가 시간을 소모했을 때 발생할 글로벌 이벤트
    public static Action<int> OnTimeConsumed;

    // 하루가 끝났을 때 발생할 글로벌 이벤트
    public static Action OnDayEnded;

    // 누군가 심야식당 조건을 만족하였을때 이벤트
    public static Action OnNightRestaurant;

    // 특정 시간 때 발동되는 이벤트
    public static Action<int> OnNpcSpecificTimeReached;
}