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
        tipPanel.SetActive(true);
    }

    public void CloseTipPanel()
    {
        tipPanel.SetActive(false);
    }
}
