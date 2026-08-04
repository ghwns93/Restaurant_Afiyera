using UnityEngine;

public class HomeSubUiCommonScript : MonoBehaviour
{
    [SerializeField] private SceneType sceneType;

    public void OnCloseUiSceneButtonClick()
    {
        SceneController.Instance.CloseUiScene(sceneType);
    }

    public void OpenSubSceneUi()
    {
        SceneController.Instance.AddtionUiScene(sceneType);                     
    }

    public void OpenNewScene()
    {
        SystemController.Instance.SetSystemPause(false);
        SceneController.Instance.LoadSubScene(sceneType);
    }

    public void GoToSleep()
    {
        TimeBase.Instance.GoToSleep(false);


    }
}
