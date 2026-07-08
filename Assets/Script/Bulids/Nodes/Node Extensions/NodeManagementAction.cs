using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Interaction/NodeManagementAction")]
public class NodeManagementAction : NpcInteractionBase
{
    [SerializeField] private List<NpcInteractionBase> subNpcInteractionBase;

    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        List<NpcInteractionBase> unlockedTalk = new List<NpcInteractionBase>();

        foreach(var talk in subNpcInteractionBase)
        {
            if (talk.CanActivate())
            {
                unlockedTalk.Add(talk);
            }
        }

        NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, actor);

        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);
    }
}
