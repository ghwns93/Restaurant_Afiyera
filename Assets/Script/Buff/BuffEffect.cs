using UnityEngine;

public abstract class BuffEffect : ScriptableObject
{
    [Header("버프 기본 정보")]
    public string buffName;
    public int duration; // 버프 지속 시간 (일 단위)
    public Sprite buffIcon;
    public float buffAmount;

    [TextArea]
    public string description; // 버프 설명

    // 버프가 적용될 때 호출 (플레이어 컴포넌트를 매개변수로 받음)
    public abstract void Apply(GameObject target);

    // 버프가 끝날 때 호출 (스탯 원복 등)
    public abstract void Remove(GameObject target);
}