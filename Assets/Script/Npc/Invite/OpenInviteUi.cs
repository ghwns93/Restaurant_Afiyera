using UnityEngine;

public class OpenInviteUi : ButtonSystemController
{
    [SerializeField] private TownType townType;

    public void OpenInviteUI()
    {
        SystemController.Instance.SetSystemPause(false);
        TownNpcListUI.Instance.OpenUI(ScreenTransitionManager.Instance.CurrentTownType);
    }
}
