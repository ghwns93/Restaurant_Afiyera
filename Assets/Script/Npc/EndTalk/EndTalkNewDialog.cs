using UnityEngine;

[CreateAssetMenu(fileName = "NewEndTalkEvent", menuName = "Quest/EndTalk/Tutorial")]
public class EndTalkNewDialog : EndTalkBase
{
    [SerializeField] private NpcSpecialEventData nextData;

    public override void EndTalk()
    {
        NpcSpecialEventManager.Instance.SetSelectedEventId(nextData);

        SceneController.Instance.LoadSubScene(SceneType.NightRestaurant);
    }
}
