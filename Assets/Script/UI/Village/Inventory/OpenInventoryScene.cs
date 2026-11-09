using UnityEngine;

public class OpenInventoryScene : ButtonSystemController
{
    public void OpenInventory()
    {
        SystemController.Instance.SetSystemPause(false);
        InventorySceneManager.Instance.OpenUI();
    }
}
