using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/NodeHarvestAction")]
public class NodeHarvestAction : NpcInteractionBase
{
    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        BasicNode bn = actor.GetComponent<BasicNode>();

        bn.HarvestAction();

        NpcInteractionManager.Instance.CompleteQuest(targetNpcId, this, questType);
    }
}
