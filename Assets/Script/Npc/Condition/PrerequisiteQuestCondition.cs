using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/PrerequisiteQuest")]
public class PrerequisiteQuestCondition : QuestCondition
{
    // 인스펙터에서 "어떤 퀘스트가 먼저 깨져야 하는지" 드래그 앤 드롭으로 지정
    [SerializeField] private NpcInteractionBase targetRequiredQuest;

    public override bool IsMet()
    {
        if (targetRequiredQuest == null) return true;

        // QuestManager에게 이 선행 퀘스트가 완료되었는지 물어봅니다.
        return NpcInteractionManager.Instance.IsQuestCompleted(targetRequiredQuest);
    }
}
