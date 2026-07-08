using System.Collections.Generic;
using UnityEngine;

public class NpcInteractionManager : MonoBehaviour
{
    public static NpcInteractionManager Instance;

    // 퀘스트 ID와 완료 여부를 저장
    private Dictionary<string, QuestType> questComplete = new Dictionary<string, QuestType>();

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteQuest(string targetId, NpcInteractionBase quest, QuestType questState)
    {
        string key = targetId + "_" + quest.dialogueKey;

        if (!questComplete.ContainsKey(key))
        {
            questComplete.Add(key, questState);
        }
    }

    // 조건 충족 여부 확인
    public bool IsQuestCompleted(string targetId, NpcInteractionBase quest)
    {
        string key = targetId + "_" + quest.dialogueKey;

        if (questComplete.ContainsKey(key))
        {
            return true;
        }

        return false;
    }
}
