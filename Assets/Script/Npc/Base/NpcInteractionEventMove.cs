using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Interaction/NpcInteractionEventMove")]
public class NpcInteractionEventMove : NpcInteractionBase, IQuestEventListener
{
    [SerializeField] private Vector3 movePos;
    [SerializeField] private Sprite changeSprite;

    [TextArea(3, 10)]
    public string talkContent; // 대화 내용

    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        NpcTalkUIManager.Instance.SetTalkText(talkContent);
    }

    public void OnEvaluateState(GameObject ownerNpc)
    {
        ownerNpc.transform.position = movePos;
    }
}
