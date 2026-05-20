using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Talk")]
public class NpcInteractionTalk : NpcInteractionBase
{
    [TextArea(3, 10)]
    public string talkContent; // 대화 내용

    // 로직: 대화 UI를 열고, dialogueKey에 해당하는 대화를 표시
    public override void Execute(GameObject actor)
    {
        NpcTalkUIManager.Instance.SetTalkText(talkContent);
    }
}
