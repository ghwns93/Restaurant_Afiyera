using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [SerializeField] private int privateNowDay = 1;

    [SerializeField] private float oneDayDurationInSeconds = 300; // 하루가 몇 초인지 설정 (테스트용)

    private float nowOneDayTime = 0;
    private float timePerOnce;

    private int nowTime = 0;

    private const float realSecondsPerDay = 86400; // 실제 하루의 초 수 (24시간 * 60분 * 60초)
    private const int secondsPerHour = 3600; // 한 시간의 초 수

    private Coroutine ClockCoroutine;

    public int NowDay => privateNowDay;

    private void Awake()
    {
        Instance = this;

        timePerOnce = realSecondsPerDay / oneDayDurationInSeconds; // 실제 시간 대비 게임 내 시간 비율 계산
    }

    private void Start()
    {
        if(ClockCoroutine == null)
        {
            ClockCoroutine = StartCoroutine(CountTime());
        }
    }

    private void OnDisable()
    {
        if(ClockCoroutine != null)
        {
            StopCoroutine(ClockCoroutine);
            ClockCoroutine = null;
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

    // 날짜가 넘어갈 때 호출하는 함수 (예: 자고 일어났을 때)
    public void NextDay()
    {
        privateNowDay++;
        //Debug.Log($"새로운 날이 밝았습니다! 현재 날짜: {privateNowDay}일");

        ProcessDayActions();
    }

    private void ProcessDayActions()
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
}