using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/SetNewDayAndLoadSceneAction")]
public class SetNewDayAndLoadSceneAction : NpcInteractionBase
{
    [SerializeField] private SceneType enterSceneType;
    [SerializeField] private bool isNewDay;

    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        SystemController.Instance.SetSystemPause(false);

        TimeBase.Instance.IsNewDay = isNewDay;

        SceneController.Instance.LoadSubScene(enterSceneType);
        //SceneController.Instance.AddtionUiScene(SceneType.Home);
    }
}
