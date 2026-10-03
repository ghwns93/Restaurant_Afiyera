using UnityEngine;

public class OpenInviteUi : MonoBehaviour
{
    [SerializeField] private TownType townType;

    public void OpenInviteUI()
    {
        SystemController.Instance.SetSystemPause(false);
        TownNpcListUI.Instance.OpenUI(townType);
    }
}
