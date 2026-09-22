using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEventUnlockAction", menuName = "Interaction/SpecialEventUnlockAction")]
public class SpecialEventUnlockAction : NpcInteractionBase
{
    [Header("해금시킬 히든 이벤트")]
    [SerializeField] private NpcSpecialEventData unlockEventData;

    public override void Execute(GameObject actor)
    {
        // 1. 퀘스트 완료 처리
        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);

        // 2. 히든 이벤트 매니저에 해금 요청 전달
        if (unlockEventData != null)
        {
            NpcSpecialEventManager.Instance.UnlockEvent(unlockEventData);
        }

        NpcTalkUIManager.Instance.EndTalk();
    }
}