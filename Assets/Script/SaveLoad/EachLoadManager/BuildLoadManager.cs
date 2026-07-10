using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BuildLoadManager : TempManagerBase<BuildLoadManager, List<BuildingData>>
{
    protected override List<BuildingData> GetMyDataFromMaster(SaveData masterSaveData)
    {
        return masterSaveData.buildings;
    }

    protected override void SetMyDataToMaster(SaveData masterSaveData, List<BuildingData> currentTempData)
    {
        masterSaveData.buildings = currentTempData;
    }

    protected override void OnDataInitialized(List<BuildingData> initializedData)
    {
        // [로드 후 행동] 이제 부모가 챙겨다 준 initializedData(tempValues)를 가지고 
        // 실제 프리팹을 맵에 스폰하거나 필요한 오브젝트에 Action을 쏘면 됩니다!
        foreach (var building in initializedData)
        {
            Debug.Log($"건물 배치 재현: {building.id}");

            var prefab = BuildDicManager.Instance.GetData(building.id);

            if (prefab != null)
            {
                BuildManager.Instance.PlaceNode(building.position, prefab, false);
            }
            else
            {
                Debug.LogWarning($"건물 ID {building.id}에 해당하는 프리팹을 찾을 수 없습니다.");
            }
        }
    }

    // 인게임에서 건물을 지을 때는 부모의 tempValues에 접근해서 추가만 해주면 끝!
    public void BuildNewStructure(BuildingData newData)
    {
        tempValues.Add(newData);
    }
}
