using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoLoadManager : TempManagerBase<PlayerInfoLoadManager, PlayerData>
{
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (MasterSaveManager.Instance != null && MasterSaveManager.Instance.currentSaveData != null)
        {
            MasterSaveManager.Instance.currentSaveData.playerInfo = tempValues;

            //Debug.Log($"PlayerInfoLoadManager position saved: {tempValues.lastPosition}");
        }
    }


    protected override PlayerData GetMyDataFromMaster(SaveData masterSaveData)
    {
        return masterSaveData.playerInfo;
    }

    protected override void SetMyDataToMaster(SaveData masterSaveData, PlayerData currentTempData)
    {
        masterSaveData.playerInfo = currentTempData;
    }

    protected override void OnDataInitialized(PlayerData initializedData)
    {
        // [로드 후 행동] 이제 부모가 챙겨다 준 initializedData(tempValues)를 가지고 
        // 실제 프리팹을 맵에 스폰하거나 필요한 오브젝트에 Action을 쏘면 됩니다!
        GameObject player = GameObject.FindWithTag("Player");

        //Debug.Log($"PlayerInfoLoadManager position loaded: {initializedData.lastPosition}");
        player.transform.position = initializedData.lastPosition;
        Camera.main.transform.position = initializedData.lastCameraPosition;
    }

    public void NewDataStructure(PlayerData newData)
    {
        tempValues = newData;
    }
}
