using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/HomeEnterAction")]
public class HomeEnterAction : NpcInteractionBase
{
    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        SystemController.Instance.SetSystemPause(false);

        TimeBase.Instance.IsNewDay = false;

        SceneController.Instance.LoadSubScene(SceneType.Home);
        //SceneController.Instance.AddtionUiScene(SceneType.Home);
    }
}
