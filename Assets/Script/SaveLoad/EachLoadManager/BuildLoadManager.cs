using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildLoadManager : TempManagerBase<BuildLoadManager, List<BuildingData>>
{
    protected override List<BuildingData> GetMyDataFromMaster(SaveData masterSaveData)
    {
        return new List<BuildingData>(masterSaveData.buildings);
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
            //Debug.Log($"건물 배치 재현: {building.id}");

            var prefab = BuildDicManager.Instance.GetData(building.id);

            if (prefab != null)
            {
                BuildManager.Instance.LoadedNode(building, prefab);
            }
            else
            {
                //Debug.LogWarning($"건물 ID {building.id}에 해당하는 프리팹을 찾을 수 없습니다.");
            }
        }
    }

    // 인게임에서 건물을 지을 때는 부모의 tempValues에 접근해서 추가만 해주면 끝!
    public void NewDataStructure(BuildingData newData)
    {
        var duplicate = FineDupData(newData);

        if(duplicate == -1)
        {
            tempValues.Add(newData);
        }
        else
        {
            //Debug.Log("중복 있음!");
            tempValues[duplicate] = newData; // 중복된 데이터가 있으면 기존 데이터를 업데이트
        }
    }

    public void DeleteBuildData(BuildingData dataToDelete)
    {
        var duplicate = FineDupData(dataToDelete);
        if (duplicate != -1)
        {
            tempValues.RemoveAt(duplicate);
        }
        else
        {
            //Debug.LogWarning($"삭제하려는 건물 데이터가 존재하지 않습니다. ID: {dataToDelete.id}, Position: {dataToDelete.position}");
        }
    }

    private int FineDupData(BuildingData newData)
    {
        for (int i = 0; i < tempValues.Count; i++)
        {
            var existingData = tempValues[i];

            if (existingData.id == newData.id && existingData.position == newData.position)
            {
                return i; // 중복된 데이터의 인덱스를 반환
            }
        }
        return -1; // 중복된 데이터가 없음을 나타냄
    }
}
