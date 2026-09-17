using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NpcLoadManager : TempManagerBase<NpcLoadManager, List<NpcData>>
{
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (MasterSaveManager.Instance != null && MasterSaveManager.Instance.currentSaveData != null)
        {
            MasterSaveManager.Instance.currentSaveData.npcs = tempValues;

            Debug.Log($"NpcLoadManager: {tempValues.Count}개의 npc 데이터를 저장했습니다.");
        }
    }

    protected override List<NpcData> GetMyDataFromMaster(SaveData masterSaveData)
    {
        return masterSaveData.npcs;
    }

    protected override void SetMyDataToMaster(SaveData masterSaveData, List<NpcData> currentTempData)
    {
        masterSaveData.npcs = currentTempData;
    }

    protected override void OnDataInitialized(List<NpcData> initializedData)
    {

    }

    public void NewDataStructure(NpcData newData)
    {
        foreach (var npc in tempValues)
        {
            if (npc.npcId == newData.npcId)
            {
                //Debug.LogWarning($"NpcLoadManager: 이미 존재하는 NPC ID({newData.npcId})입니다. 기존 데이터를 덮어씁니다.");
                tempValues.Remove(npc);
                break;
            }
        }

        tempValues.Add(newData);

        Debug.Log($"NpcLoadManager: 새로운 NPC 데이터({newData.npcId})를 추가했습니다. 현재 NPC 수: {tempValues.Count}");
    }

    public NpcData GetNpcDataById(string npcId)
    {
        if(tempValues == null)
        {
            //Debug.Log("NpcLoadManager: tempValues가 초기화되지 않았습니다.");
            return null;
        }

        if(tempValues.Count == 0)
        {
            //Debug.Log("NpcLoadManager: tempValues가 비어있습니다.");
            return null;
        }

        return tempValues.Find(npc => npc.npcId == npcId);
    }
}
