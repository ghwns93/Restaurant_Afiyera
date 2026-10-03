using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/EnterNightRestaurantCondition")]
public class EnterNightRestaurantCondition : QuestCondition
{
    // 인스펙터에서 "초대 완료" 지정
    [SerializeField] private bool needInviteCustomer;

    public override bool IsMet(string targetId)
    {
        var npcData = NpcSpecialEventManager.Instance.GetSelectedId();

        // needInviteCustomer 값과 (npcData가 존재하는지 여부)가 일치할 때 true 반환
        return needInviteCustomer == (npcData != null);
    }
}
