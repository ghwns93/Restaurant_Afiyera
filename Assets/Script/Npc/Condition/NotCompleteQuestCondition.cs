using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/NotCompleteQuestCondition")]
public class NotCompleteQuestCondition : QuestCondition
{
    // 인스펙터에서 "어떤 퀘스트가 먼저 깨지면 안되는지" 드래그 앤 드롭으로 지정
    [SerializeField] private List<NpcInteractionBase> targetRequiredQuest;

    public override bool IsMet(string targetId)
    {
        if (targetRequiredQuest == null) return true;

        bool result;

        foreach (var quest in targetRequiredQuest)
        {
            //완료 되면 안되는 조건 전부 확인
            result = !NpcInteractionManager.Instance.IsQuestCompleted(targetId, quest);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }
}
