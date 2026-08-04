using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcInteractionManager : MonoBehaviour
{
    public static NpcInteractionManager Instance;

    private readonly Func<string,string,string> makeKey = (k1,k2) => $"{k1}_{k2}";

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

    public void LoadQuestData(List<QuestData> questDataList)
    {
        questComplete.Clear();

        foreach (var questData in questDataList)
        {
            if (!questComplete.ContainsKey(questData.id))
            {
                questComplete.Add(questData.id, questData.isCompleted);
            }
        }
    }

    public void CompleteQuest(string targetId, NpcInteractionBase quest, QuestType questState)
    {
        string key = makeKey(targetId , quest.dialogueKey);

        if (!questComplete.ContainsKey(key))
        {
            questComplete.Add(key, questState);

            SaveQuestData();
        }
    }

    // 조건 충족 여부 확인
    public bool IsQuestCompleted(string targetId, NpcInteractionBase quest)
    {
        string key = makeKey(targetId, quest.dialogueKey);

        if (questComplete.ContainsKey(key))
        {
            return true;
        }

        return false;
    }

    public void ResetQuest(string targetId, NpcInteractionBase quest)
    {
        string key = makeKey(targetId, quest.dialogueKey);

        if (questComplete.ContainsKey(key))
        {
            questComplete.Remove(key);

            SaveQuestData();
        }
        else
        {
            Debug.Log("일치하는 퀘스트가 없습니다.");
        }
    }

    private void SaveQuestData()
    {
        var questDataList = new List<QuestData>(questComplete.Select(kvp => new QuestData { id = kvp.Key, isCompleted = kvp.Value }));

        QuestLoadManager.Instance.NewDataStructure(questDataList);
    }
}
