using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerBuff", menuName = "WorkerBuffs/CookSpeed")]
public class WorkerCookSpeed : BuffEffect
{
    public override void Apply(GameObject target)
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.CookSpeed, buffAmount);
        }
    }

    public override void Remove(GameObject target)
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.CookSpeed, (buffAmount * -1));
        }
    }
}
