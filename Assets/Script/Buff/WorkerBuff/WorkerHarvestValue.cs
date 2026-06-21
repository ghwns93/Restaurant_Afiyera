using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerBuff", menuName = "WorkerBuffs/HarvestValue")]
public class WorkerHarvestValue : BuffEffect
{
    public override void Apply(GameObject target)
    {
        if(StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.HarvestValue, buffAmount);
        }
    }

    public override void Remove(GameObject target)
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.SetState(WorkerStateType.HarvestValue, (buffAmount * -1));
        }
    }
}
