using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeBase : MonoBehaviour
{
    [SerializeField] private int privateNowDay = 1;

    [SerializeField] protected int workTime = 10;
    [SerializeField] protected int nightOpenTime = 22;
    [SerializeField] protected int sleepTime = 24;

    public static TimeBase Instance;

    protected bool todayNightRestaurantHasOpen = false; // 오늘 밤 식당이 열리는지 여부
    protected bool todayNightRestaurantIsWorked = false; // 오늘 밤 식당이 열렸는지 여부

    protected const float realSecondsPerDay = 86400; // 실제 하루의 초 수 (24시간 * 60분 * 60초)
    protected const int secondsPerHour = 3600; // 한 시간의 초 수
    protected const int secondsPerMinute = 60; // 일 분의 초 수

    protected bool isWorking = false;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        TimeEvents.OnNightRestaurant += NightRestaurantOpen;
    }

    private void OnDisable()
    {
        TimeEvents.OnNightRestaurant -= NightRestaurantOpen;
    }

    public void NightRestaurantOpen()
    {
        todayNightRestaurantHasOpen = true;
    }

    // 날짜가 넘어갈 때 호출하는 함수 (예: 자고 일어났을 때)
    public virtual void NextDay()
    {
        privateNowDay++;
        //Debug.Log($"새로운 날이 밝았습니다! 현재 날짜: {privateNowDay}일");

        ProcessDayActions();

        isWorking = false; // 하루가 끝나면 작업 상태를 초기화

        // 밤 식당 상태 초기화
        todayNightRestaurantHasOpen = false;
        todayNightRestaurantIsWorked = false; 

        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.DayCheck(1); // 버프 지속 시간 감소
        }
    }

    protected virtual void ProcessDayActions()
    {
        // BuildManager에 있는 모든 노드 리스트를 가져옵니다.
        var allNodes = BuildManager.Instance.GetAllNodes();

        // 이번 '날'에 이미 행동을 완료한 그룹들을 저장 (중복 방지)
        HashSet<NodeGroup> processedGroups = new HashSet<NodeGroup>();

        foreach (var node in allNodes)
        {
            // 1. 날짜 주기 체크 (NowDay % DayCount == 0)
            if (privateNowDay % node.DayCount != 0) continue;

            // 2. 그룹 소속 여부 확인
            if (node.ParentGroup != null)
            {
                // 이미 이 그룹이 오늘 행동을 했다면 스킵
                if (processedGroups.Contains(node.ParentGroup)) continue;

                // 그룹 전체 행동 실행 및 기록
                node.ParentGroup.ExecuteGroupDayAction();
                processedGroups.Add(node.ParentGroup);
            }
            else
            {
                // 그룹이 없는 단독 노드라면 바로 실행
                node.DayAction();
            }
        }
    }

    public void TempNightOpen()
    {
        TimeEvents.OnNightRestaurant?.Invoke();
    }

    public abstract void SetNowTime(int hour);

    protected abstract void GoToWork();
    protected abstract void GoToNightWork();
    protected abstract void GoToSleep();
}
