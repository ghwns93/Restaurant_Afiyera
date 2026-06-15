using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/NodeManagementCountAction")]
public class NodeManagementCountAction : NpcInteractionBase
{
    // 로직: 직접적인 동작 발현
    public override void Execute(GameObject actor)
    {
        BasicNode bn = actor.GetComponent<BasicNode>();

        bn.ManagementCountAction();
    }
}
