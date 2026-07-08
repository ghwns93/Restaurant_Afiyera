using UnityEngine;

public class TempCookAndNightCookScene : MonoBehaviour
{
    public void CookEnd()
    {
        TimeBase.Instance.SetNowTime(17);
        TimeBase.Instance.nowTimeState = TimeState.Night;

        SceneController.Instance.LoadSubScene(SceneType.Village);
    }

    public void NightCookEnd()
    {
        TimeBase.Instance.GoToSleep();
    }
}
