using UnityEngine;

public class TimeSettingManager : MonoBehaviour
{
    private void Awake()
    {
        SystemController.Instance.SetSystemPause(true);
    }

    private void OnDisable()
    {
        SystemController.Instance.SetSystemPause(false);
    }
}
