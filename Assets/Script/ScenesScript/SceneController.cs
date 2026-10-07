using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    HomeUi,
    ShopCody,
    ShopFlavoring,
    InviteCustomer,
    inventory,
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

    public bool optionOpened = false;

    [Header("Loading UI")]
    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public Slider progressBar;

    private string currentSubScene;

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

    public void OptionSceneOpenOrClose()
    {
        if (optionOpened == false)
        {
            SystemController.Instance.SetSystemPause(false);
            AddtionUiScene(SceneType.Option);
        }
        else
        {
            SystemController.Instance.SetSystemPause(true);
            CloseUiScene(SceneType.Option);
        }

        optionOpened = !optionOpened;
    }

    public void LoadSceneAtButton(int sceneIndex)
    {
        SceneType type = (SceneType)sceneIndex;

        LoadSubScene(type);
    }

    public void LoadSubScene(SceneType type)
    {
        //// 1. 이미 켜져 있는 서브 씬이 있다면 먼저 언로드
        //if (!string.IsNullOrEmpty(currentSubScene))
        //{
        //    SceneManager.UnloadSceneAsync(currentSubScene);
        //}

        //// 2. 리스트에서 맞는 씬 이름을 찾아서 로드
        //SceneData data = sceneList.Find(s => s.type == type);

        //if (data.sceneName != null)
        //{
        //    currentSubScene = data.sceneName;
        //    // Additive 모드로 로드하여 메인 씬을 유지함
        //    SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
        //}

        StartCoroutine(LoadSubSceneRoutine(type));
    }

    private IEnumerator LoadSubSceneRoutine(SceneType type)
    {
        // 1. 로딩창 켜기
        if (loadingPanel != null) loadingPanel.SetActive(true);

        // 2. 이미 켜져 있는 서브 씬이 있다면 언로드 후 완료될 때까지 대기
        if (!string.IsNullOrEmpty(currentSubScene))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSubScene);
            if (unloadOp != null)
            {
                while (!unloadOp.isDone)
                {
                    yield return null;
                }
            }
        }

        // 3. 리스트에서 맞는 씬 이름 찾기
        SceneData data = sceneList.Find(s => s.type == type);

        if (data.sceneName != null)
        {
            currentSubScene = data.sceneName;

            // 4. Additive 모드로 비동기 로드 시작
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = true; // 바로 활성화할 경우

            // 5. 로딩 진행도 반영
            while (!loadOp.isDone)
            {
                // progress는 0부터 0.9까지만 가므로 1.0 기준으로 보정하려면 아래와 같이 처리
                float progress = Mathf.Clamp01(loadOp.progress / 0.9f);

                if (progressBar != null)
                {
                    progressBar.value = progress;
                }

                yield return null;
            }
        }

        // 6. 로딩 완료 후 로딩창 끄기
        if (loadingPanel != null) loadingPanel.SetActive(false);
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