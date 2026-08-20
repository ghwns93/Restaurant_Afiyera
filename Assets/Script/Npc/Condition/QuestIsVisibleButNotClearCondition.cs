using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/QuestIsVisibleButNotClear")]
public class QuestIsVisibleButNotClearCondition : QuestCondition
{
    // 활성화가 되어있지만 아직 클리어는 안되었을때 조건 확인용
    [SerializeField] private List<NpcInteractionBase> targetRequiredQuest;

    public override bool IsMet(string targetId)
    {
        if (targetRequiredQuest == null) return true;

        bool result;

        foreach(var quest in targetRequiredQuest)
        {
            var newInteraction = Instantiate(quest);

            newInteraction.targetNpcId = targetId;

            // 활성화 되는지 체크
            if (newInteraction.CanActivate())
            {
                //아직 안깨졌다면 가리기
                result = NpcInteractionManager.Instance.IsQuestCompleted(targetId, newInteraction);
                if (!result)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
