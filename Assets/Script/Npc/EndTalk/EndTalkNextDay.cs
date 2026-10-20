using UnityEngine;

[CreateAssetMenu(fileName = "NewEndTalkEvent", menuName = "Quest/EndTalk/NextDay")]
public class EndTalkNextDay : EndTalkBase
{
    public override void EndTalk()
    {
        NpcSpecialEventManager.Instance.ResetEvent(); 
        TimeBase.Instance.GoToSleep(false);    }
}
