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
}
