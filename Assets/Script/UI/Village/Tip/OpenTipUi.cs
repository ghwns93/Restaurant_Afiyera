using UnityEngine;

public class OpenTipUi : MonoBehaviour
{
    [SerializeField]
    private GameObject tipPanel;

    void Start()
    {
        tipPanel.SetActive(false);
    }

    public void OpenTipPanel()
    {
        SystemController.Instance.SetSystemPause(false);

        tipPanel.SetActive(true);
    }

    public void CloseTipPanel()
    {
        SystemController.Instance.SetSystemPause(true);

        tipPanel.SetActive(false);
    }
}
