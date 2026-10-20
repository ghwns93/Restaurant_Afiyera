using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private NpcSpecialEventData introData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartIntro()
    {
        NpcSpecialEventManager.Instance.SetSelectedEventId(introData);

        SceneController.Instance.LoadSubScene(SceneType.NightRestaurant);
    }
}
