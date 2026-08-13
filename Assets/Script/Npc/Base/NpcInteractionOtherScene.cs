using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Quest/Interaction/OtherScene")]
public class NpcInteractionOtherScene : NpcInteractionBase
{
    public SceneType targetScene; // 이동할 씬 타입

    // 로직: 다른 씬으로 이동
    public override void Execute(GameObject actor)
    {
        //SceneController.Instance.LoadSubScene(targetScene);
        SceneController.Instance.LoadSubScene(SceneType.Shop);
    }
}
