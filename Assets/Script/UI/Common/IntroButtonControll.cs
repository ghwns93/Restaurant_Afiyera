using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroButtonControll : MonoBehaviour
{
    [SerializeField] GameObject _PopUp;

    public void SetPopUp()
    {
        _PopUp.SetActive(!_PopUp.activeSelf);
    }

    public void ActivePopUp()
    {
        _PopUp.SetActive(true);
    }

    public void ClosePopUp()
    {
        _PopUp.SetActive(false);
    }

    public void OpenOption()
    {
        SceneController.Instance.OptionSceneOpenOrClose();
    }

    public void StartGame()
    {
        //SceneController.Instance.LoadSubScene(SceneType.Village);

        TutorialManager.Instance.StartIntro();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
