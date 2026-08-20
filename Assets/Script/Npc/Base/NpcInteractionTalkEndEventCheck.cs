using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Interaction/TalkEndEventCheck")]
public class NpcInteractionTalkEndEventCheck : NpcInteractionBase
{
    public NpcInteractionBase saveInteraction;

    // 로직: 대화 UI를 열고, dialogueKey에 해당하는 대화를 표시
    public override void Execute(GameObject actor)
    {
        NpcTalkUIManager.Instance.EndTalk();
        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, saveInteraction, questType);
    }
}
