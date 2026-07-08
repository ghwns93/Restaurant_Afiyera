using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/CooldownOrLock")]
public class QuestLockCondition : QuestCondition
{
    private bool isLocked = false;

    public void SetLock(bool value) => isLocked = value;

    public override bool IsMet()
    {
        // 락이 걸려있으면(true) 조건 만족 못함(false)
        return !isLocked;
    }
}
