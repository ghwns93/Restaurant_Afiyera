using UnityEngine;
using UnityEngine.UI;

public class ButtonSystemController : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        SystemController.OnSystemStateChanged += SystemActionFunc;
    }

    private void OnDisable()
    {
        SystemController.OnSystemStateChanged -= SystemActionFunc;
    }

    private void SystemActionFunc(bool pause)
    {
        image.raycastTarget = pause;
    }
}
