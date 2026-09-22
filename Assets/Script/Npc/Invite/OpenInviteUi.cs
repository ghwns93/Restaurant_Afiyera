using UnityEngine;

public class OpenInviteUi : MonoBehaviour
{
    [SerializeField] private TownType townType;

    public void OpenInviteUI()
    {
        TownNpcListUI.Instance.OpenUI(townType);
    }
}
