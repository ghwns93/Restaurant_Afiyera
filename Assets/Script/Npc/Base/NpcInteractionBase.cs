using System.Collections.Generic;
using UnityEngine;

public abstract class NpcInteractionBase : ScriptableObject, NpcInteractionInterface
{
    public QuestType questType; // 인스펙터에서 선택 (반복 인지 한번인지 선택)

    public QuestInteractionType questInteractionType; // 인스펙터에서 선택 (일반/수확/관리 구분)

    public string dialogueKey; // 대화 테이블 키

    public string targetNpcId; // 이 퀘스트를 수행할 NPC의 ID

    [Header("퀘스트 조건")]
    // 이 퀘스트가 활성화되기 위해 만족해야 하는 조건들 (낮밤, 선행퀘 등)
    [SerializeField] private List<QuestCondition> unlockConditions;

    public bool CanActivate()
    {
        if (unlockConditions == null || unlockConditions.Count == 0)
            return true;

        foreach (var condition in unlockConditions)
        {
            if (!condition.IsMet(targetNpcId)) return false; // 하나라도 만족 못하면 락
        }

        return true;
    }

    // 조건들이 QuestSO 내부를 뒤질 때 편하게 쓰기 위한 헬퍼 함수
    public T GetCondition<T>() where T : QuestCondition
    {
        return unlockConditions.Find(c => c is T) as T;
    }

    public abstract void Execute(GameObject actor);
}

public enum QuestType { Repeat, Once }

public enum QuestInteractionType { Talk, Harvest, Management }