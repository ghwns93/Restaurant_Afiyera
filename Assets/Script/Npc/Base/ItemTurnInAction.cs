using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Interaction/ItemQuestAction")]
public class ItemTurnInAction : NpcInteractionBase
{
    [SerializeField] private ItemData removeItemId;
    [SerializeField] private int removeCount = 1;
    [SerializeField] private int rewardGold = 100;

    [SerializeField] private List<NpcInteractionBase> choiceList; // 다음 대화 내용

    [TextArea(3, 10)]
    public string questCompleteTalkContent; // 조건 만족 시 대화 내용

    [TextArea(3, 10)]
    public string questFailTalkContent;     // 조건 미 만족 시 대화 내용

    public override void Execute(GameObject actor)
    {
        int currentCount = InventoryManager.Instance.GetItemCount(removeItemId);

        if(removeCount> currentCount)
        {
            // 조건 미 만족 시 대화 UI 표시
            NpcTalkUIManager.Instance.SetTalkText(questFailTalkContent);
            return;
        }
        else
        {
            // 조건 만족 시 아이템 차감, 보상 지급, 대화 UI 표시, 퀘스트 완료 처리

            // 1. 아이템 차감
            InventoryManager.Instance.ReduceItem(removeItemId, removeCount);

            // 2. 보상 지급
            InventoryManager.Instance.AddGold(rewardGold);

            // 3. 대화 UI 표시
            NpcTalkUIManager.Instance.SetTalkText(questCompleteTalkContent);

            // 4. 퀘스트 완료 처리
            NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);

            // 5. 다음 대화 내용 표시
            List<NpcInteractionBase> unlockedTalk = new List<NpcInteractionBase>();

            foreach (var talk in choiceList)
            {
                if (talk.CanActivate())
                {
                    var newInteraction = Instantiate(talk);

                    newInteraction.targetNpcId = targetNpcId;

                    unlockedTalk.Add(newInteraction);
                }
            }

            NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, actor);
        }
    }
}