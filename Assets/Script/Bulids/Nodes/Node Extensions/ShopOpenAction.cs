using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/ShopOpenAction")]
public class ShopOpenAction : NpcInteractionBase
{
    [SerializeField] private int openShopHour = 7;
    [SerializeField] private int openShopMinute = 0;
    [SerializeField] private int closeShopHour = 19;
    [SerializeField] private int closeShopMinute = 0;

    [Header("상점 닫았을 때 대화 내용")]
    [TextArea(3, 10)]
    public string closeShopTalkContent; // 대화 내용

    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        if (TimeBase.Instance.CheckBetweenTime(openShopHour, 0, closeShopHour, 0))
        {
            SystemController.Instance.SetSystemPause(false);

            var npcInfo = actor.GetComponent<ShopNpcScript>();

            if (npcInfo == null)
            {
                Debug.LogError("ShopOpenAction: Actor does not have ShopNpcScript component.");
                return;
            }

            NpcTalkUIManager.Instance.EndTalk();
            ShopManager.Instance.OpenShop(npcInfo.ShopItemList, npcInfo.IngredientItemList, npcInfo.MyNpcBasicInfo.npcImage);
        }
        else
        {
            string text = closeShopTalkContent
                .Replace("{startHour}", openShopHour.ToString("D2"))
                .Replace("{startMinute}", openShopMinute.ToString("D2"))
                .Replace("{endHour}", closeShopHour.ToString("D2"))
                .Replace("{endMinute}", closeShopMinute.ToString("D2"));

            NpcTalkUIManager.Instance.SetTalkText(text);
        }
    }
}
