using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/RestaurantEnterAction")]
public class RestaurantEnterAction : NpcInteractionBase
{
    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        SystemController.Instance.SetSystemPause(false);

        if (TimeBase.Instance.nowTimeState == TimeState.Day)
            SceneController.Instance.LoadSubScene(SceneType.Restaurant);

        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);
    }
}
