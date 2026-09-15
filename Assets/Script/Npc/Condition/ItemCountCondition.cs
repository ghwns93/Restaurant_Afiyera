using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/ItemCountCondition")]
public class ItemCountCondition : QuestCondition
{
    [SerializeField] private ItemData itemId;
    [SerializeField] private int requiredCount;

    public override bool IsMet(string targetId)
    {
        // 인벤토리 매니저에서 아이템 개수 확인
        int currentCount = InventoryManager.Instance.GetItemCount(itemId);
        return currentCount >= requiredCount;
    }
}