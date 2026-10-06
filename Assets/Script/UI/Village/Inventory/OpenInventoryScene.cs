using UnityEngine;

public class OpenInventoryScene : MonoBehaviour
{
    public void OpenInventory()
    {
        SystemController.Instance.SetSystemPause(false);
        InventorySceneManager.Instance.OpenUI();
    }
}
