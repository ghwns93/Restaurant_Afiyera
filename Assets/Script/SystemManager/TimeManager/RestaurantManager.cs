using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    public static RestaurantManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OpenRestaurnat()
    {
        SystemController.Instance.SetSystemPause(false);
        SceneController.Instance.AddtionUiScene(SceneType.Restaurant);
    }

    public void CloseRestaurnat()
    {
        SystemController.Instance.SetSystemPause(true);
        SceneController.Instance.CloseUiScene(SceneType.Restaurant);

        TimeBase.Instance.SetNowTime(17);
    }

    public void OpenNightRestaurnat()
    {
        SystemController.Instance.SetSystemPause(false);
        SceneController.Instance.AddtionUiScene(SceneType.NightRestaurant);
    }

    public void CloseNightRestaurnat()
    {
        SystemController.Instance.SetSystemPause(true);
        SceneController.Instance.CloseUiScene(SceneType.NightRestaurant);
    }
}
