using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    // 현재 조건이 만족했는지 검사하는 메서드
    public abstract bool IsMet();
}
