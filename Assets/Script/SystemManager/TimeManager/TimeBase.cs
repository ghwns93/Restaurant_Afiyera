using System.Collections.Generic;
using UnityEngine;

public abstract class TimeBase : MonoBehaviour
{
    [SerializeField] private int privateNowDay = 1;

    // 날짜가 넘어갈 때 호출하는 함수 (예: 자고 일어났을 때)
    public virtual void NextDay()
    {
        privateNowDay++;
        //Debug.Log($"새로운 날이 밝았습니다! 현재 날짜: {privateNowDay}일");

        ProcessDayActions();

        if(BuffManager.Instance != null)
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
}
