using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Conditions/TimeOfDay")]
public class TimeOfDayCondition : QuestCondition
{
    public TimeState requiredTime;

    public override bool IsMet()
    {
        // DayManager(가칭)에서 현재 시간이 낮인지 밤인지 체크

        if(TimeBase.Instance?.nowTimeState == requiredTime)
        {
            return true;
        }

        return false;
    }
}
