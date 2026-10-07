using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEventUnlockAction", menuName = "Interaction/SpecialEventUnlockAction")]
public class SpecialEventUnlockAction : NpcInteractionBase
{
    [Header("해금시킬 히든 이벤트")]
    [SerializeField] private NpcSpecialEventData unlockEventData;

    [Header("대화 내용")]
    [TextArea(3, 10)]
    public string talkContent; // 대화 내용

    [Header("다음 대화 선택지")]
    [SerializeField] private List<NpcInteractionBase> choiceList; // 다음 대화 선택지

    public NpcSpecialEventData UnlockEventData => unlockEventData;

    public override void Execute(GameObject actor)
    {
        NpcTalkUIManager.Instance.SetTalkText(talkContent);

        List<NpcInteractionBase> unlockedTalk = new List<NpcInteractionBase>();

        foreach (var talk in choiceList)
        {
            if (talk.CanActivate())
            {
                var newInteraction = Instantiate(talk);

                newInteraction.targetNpcId = targetNpcId;

                unlockedTalk.Add(newInteraction);
            }
        }

        NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, actor);

        // 1. 퀘스트 완료 처리
        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);

        NpcTalkUIManager.Instance.EndTalk();
    }
}