using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerBuff", menuName = "WorkerBuffs/MoveSpeed")]
public class WorkerMoveSpeed : BuffEffect
{
    public override void Apply(GameObject target)
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.MoveSpeed, buffAmount);
        }
    }

    public override void Remove(GameObject target)
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.MoveSpeed, (buffAmount * -1));
        }
    }
}
