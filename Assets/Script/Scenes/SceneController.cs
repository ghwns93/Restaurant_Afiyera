using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Intro,
    Main,
    Option,
    Farm,
    Restaurant,
    Shop,
    Village,
    NpcSelectUI,
    NpcInteractionUI,
    Home,
    NightRestaurant,
    StoryAndRecipe,
    WorkerDoWork,
    WorkerEnchant,
    HomeKitchen,
}

public class SceneController : MonoBehaviour
{
    // 인스펙터에서 씬 이름을 리스트처럼 관리할 수 있게 구성
    [System.Serializable]
    public struct SceneData
    {
        public SceneType type;
        public string sceneName;
    }

    public SceneType startScene;
    public List<SceneData> sceneList;

    private string currentSubScene;

    public bool optionOpened = false;

    internal static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSubScene(startScene);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIOpenRegistry.CanOpenOption)
            {
                OptionSceneOpenOrClose();
            }
        }
    }

    public void OptionSceneOpenOrClose()
    {
        if (optionOpened == false)
            AddtionUiScene(SceneType.Option);
        else
            CloseUiScene(SceneType.Option);

        optionOpened = !optionOpened;
    }

    public void LoadSceneAtButton(int sceneIndex)
    {
        SceneType type = (SceneType)sceneIndex;

        LoadSubScene(type);
    }

    public void LoadSubScene(SceneType type)
    {
        // 1. 이미 켜져 있는 서브 씬이 있다면 먼저 언로드
        if (!string.IsNullOrEmpty(currentSubScene))
        {
            SceneManager.UnloadSceneAsync(currentSubScene);
        }

        // 2. 리스트에서 맞는 씬 이름을 찾아서 로드
        SceneData data = sceneList.Find(s => s.type == type);

        if (data.sceneName != null)
        {
            currentSubScene = data.sceneName;
            // Additive 모드로 로드하여 메인 씬을 유지함
            SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
        }
    }

    public void CloseCurrentScene()
    {
        if (!string.IsNullOrEmpty(currentSubScene))
        {
            SceneManager.UnloadSceneAsync(currentSubScene);

            currentSubScene = null;
        }
    }

    public void AddtionUiScene(SceneType scene)
    {
        SceneData data = sceneList.Find(s => s.type == scene);

        SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
    }

    public void CloseUiScene(SceneType scene)
    {
        SceneData data = sceneList.Find(s => s.type == scene);

        Scene subScene = SceneManager.GetSceneByName(data.sceneName);

        SceneManager.UnloadSceneAsync(subScene);
    }
}