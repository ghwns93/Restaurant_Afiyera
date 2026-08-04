using UnityEngine;

public class TempOpenShop : MonoBehaviour
{
    public void OpenShop()
    {
        SystemController.Instance.SetSystemPause(false);

        SceneController.Instance.LoadSubScene(SceneType.Shop);
    }
}
