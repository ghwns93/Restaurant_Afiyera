using UnityEngine;

public class TimeSettingManager : MonoBehaviour
{
    private void Start()
    {
        if (SystemController.Instance != null)
            SystemController.Instance.SetSystemPause(true);
    }

    private void OnDisable()
    {
        if (SystemController.Instance != null)
            SystemController.Instance.SetSystemPause(false);
    }
}
