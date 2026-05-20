using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        gameObject.SetActive(true);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
