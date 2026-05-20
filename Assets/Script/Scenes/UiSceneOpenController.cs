using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSceneOpenController : MonoBehaviour
{
    public List<SceneType> openableScenes;

    private void Awake()
    {
        if(SceneController.Instance == null)
        {
            SceneManager.LoadScene("MainScene"); // MainScene으로 이동하여 SceneController가 존재하도록 함

            return;
        }

        OpenUi();
    }

    private void OpenUi()
    {
        if (openableScenes != null)

        foreach (var scene in openableScenes)
        {
            SceneController.Instance.AddtionUiScene(scene);
        }
    }

    private void OnDestroy()
    {
        if (SceneController.Instance == null) return;

        foreach (var scene in openableScenes)
        {
            //Debug.Log($"Closing UI Scene: {scene}");
            SceneController.Instance.CloseUiScene(scene);
        }
    }
}
