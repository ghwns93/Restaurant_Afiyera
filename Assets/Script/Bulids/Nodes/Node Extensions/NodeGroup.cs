using System.Collections.Generic;
using UnityEngine;

public class NodeGroup
{
    private List<IConnectable> privateMembers = new List<IConnectable>();

    public void AddMember(IConnectable member) => privateMembers.Add(member);
    public int Count => privateMembers.Count;
    public List<IConnectable> GetMembers() => privateMembers;

    public void FinalizeGroup()
    {
        if (privateMembers.Count > 0)
        {
            // 그룹 내 첫 번째 멤버의 기준에 따라 성공/실패 판단
            int required = privateMembers[0].MinConnectionCount;

            if (privateMembers.Count < required)
            {
                foreach (var m in privateMembers) m.OnConnectionFailed();
            }
            else
            {
                foreach (var m in privateMembers) m.OnConnectionSuccess(privateMembers.Count);
            }
        }
    }

    // 그룹 내의 대표로 한 명만 행동을 수행하게 함
    public void ExecuteGroupDayAction()
    {
        if (privateMembers.Count > 0)
        {
            // 첫 번째 멤버(BasicNode)의 DayAction을 실행하거나, 
            // 별도의 그룹 전용 로직을 실행합니다.
            if (privateMembers[0] is BasicNode leader)
            {
                leader.DayAction();
                Debug.Log($"그룹(크기:{privateMembers.Count})이 대표로 행동을 수행했습니다.");
            }
        }
    }
}