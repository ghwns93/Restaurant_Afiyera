using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/NodeManagementAction")]
public class NodeManagementAction : NpcInteractionBase
{
    [SerializeField] private List<NpcInteractionBase> subNpcInteractionBase;

    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        foreach(var interaction in subNpcInteractionBase)
        {
            NpcInteractionManager.Instance.InputQuest(interaction, interaction.isUnRocked);
        }

        var unlockedTalk = subNpcInteractionBase
                            .Where(interaction => NpcInteractionManager.Instance.IsQuestCompleted(interaction))
                            .ToList();

        NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, actor);
    }
}
