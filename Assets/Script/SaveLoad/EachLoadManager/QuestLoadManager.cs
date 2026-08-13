using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoadManager : TempManagerBase<QuestLoadManager, List<QuestData>>
{
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (MasterSaveManager.Instance != null && MasterSaveManager.Instance.currentSaveData != null)
        {
            MasterSaveManager.Instance.currentSaveData.quests = tempValues;

            FileLogger.Log($"QuestLoadManager: {tempValues.Count}개의 퀘스트 데이터를 저장했습니다.");
        }
    }


    protected override List<QuestData> GetMyDataFromMaster(SaveData masterSaveData)
    {
        return masterSaveData.quests;
    }

    protected override void SetMyDataToMaster(SaveData masterSaveData, List<QuestData> currentTempData)
    {
        masterSaveData.quests = currentTempData;
    }

    protected override void OnDataInitialized(List<QuestData> initializedData)
    {
        // [로드 후 행동] 이제 부모가 챙겨다 준 initializedData(tempValues)를 가지고 
        // 실제 프리팹을 맵에 스폰하거나 필요한 오브젝트에 Action을 쏘면 됩니다!
        NpcInteractionManager.Instance.LoadQuestData(initializedData);
    }

    public void NewDataStructure(List<QuestData> newData)
    {
        tempValues = newData;
    }
}
