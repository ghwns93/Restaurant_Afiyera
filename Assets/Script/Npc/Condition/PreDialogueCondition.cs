using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/PreDialogueCondition")]
public class PreDialogueCondition : QuestCondition
{
    // 인스펙터에서 "어떤 스토리가 먼저 깨져야 하는지" 드래그 앤 드롭으로 지정
    [SerializeField] private SpecialEventUnlockAction targetDialogue;

    public override bool IsMet(string targetId)
    {
        return NpcSpecialEventManager.Instance.IsEventSeen(targetDialogue.UnlockEventData.name);
    }
}
