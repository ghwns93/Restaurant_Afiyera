using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/PrerequisiteQuest")]
public class PrerequisiteQuestCondition : QuestCondition
{
    // 인스펙터에서 "어떤 퀘스트가 먼저 깨져야 하는지" 드래그 앤 드롭으로 지정
    [SerializeField] private List<NpcInteractionBase> targetRequiredQuest;

    [Header("다른 NPC의 퀘스트를 확인할 경우, 해당 NPC id 입력")]
    [SerializeField] private string otherNpcId = "";

    public override bool IsMet(string targetId)
    {
        if (targetRequiredQuest == null) return true;

        if (otherNpcId.Trim() != "")
        {
            targetId = otherNpcId;
        }

        bool result;

        foreach(var quest in targetRequiredQuest)
        {
            //선행 조건 전부 확인
            result = NpcInteractionManager.Instance.IsQuestCompleted(targetId, quest);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }
}
