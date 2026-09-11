using UnityEngine;

public class CookingPopUpButon : MonoBehaviour
{
    [SerializeField] GameObject _panel;

    public void Pop()
    {
        _panel.SetActive(!_panel.activeSelf);
    }
}
