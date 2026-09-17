using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/ShopOpenAction")]
public class ShopOpenAction : NpcInteractionBase
{
    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        SystemController.Instance.SetSystemPause(false);

        var npcInfo = actor.GetComponent<ShopNpcScript>();

        if(npcInfo == null)
        {
            Debug.LogError("ShopOpenAction: Actor does not have ShopNpcScript component.");
            return;
        }

        ShopManager.Instance.OpenShop(npcInfo.ShopItemList, npcInfo.npcImage);
    }
}
