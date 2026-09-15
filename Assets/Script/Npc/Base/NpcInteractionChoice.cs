using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Interaction/Choice")]
public class NpcInteractionChoice : NpcInteractionBase
{
    [TextArea(3, 10)]
    public string talkContent; // 대화 내용

    [SerializeField] private List<NpcInteractionBase> choiceList;

    // 로직: 대화 UI를 열고, dialogueKey에 해당하는 대화를 표시
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

        Debug.Log($"[NpcInteractionChoice] targetNpcId: {targetNpcId}");

        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);
    }
}
