using UnityEngine;

[CreateAssetMenu(fileName = "NewEndTalkEvent", menuName = "Quest/EndTalk/MoveNewScene")]
public class EndTalkMoveNewScene : EndTalkBase
{
    [SerializeField] private SceneType nextScene;

    public override void EndTalk()
    {
        SceneController.Instance.LoadSubScene(nextScene);
    }
}
