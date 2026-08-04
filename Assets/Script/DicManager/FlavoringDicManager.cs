using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlavoringDicManager : BaseDicManager<FlavoringDicManager,int, GameObject>
{
    protected override int GetKey(GameObject data)
    {
        var flavoringData = data.GetComponent<FlavoringData>();

        return flavoringData.ItemData.id;
    }

    public List<GameObject> GetTypeValue(ItemType bs)
    {
        List<GameObject> resultList = new List<GameObject>();
        foreach (var data in dataList)
        {
            var flavoringData = data.GetComponent<FlavoringData>();

            if(flavoringData == null)
            {
                Debug.LogError($"FlavoringData component is missing on GameObject: {data.name}");
                continue;
            }

            if (flavoringData.ItemData.itemType == bs)
            {
                resultList.Add(data);
            }
        }
        return resultList;
    }

    public ItemData GetNodeIdByUnlockID(string unlockID)
    {
        foreach (var data in dataList)
        {
            var unlockComponent = data.GetComponent<ShopUnlockableItem>();

            if (unlockComponent != null && unlockComponent.UnlockID == unlockID)
            {
                var nodeComponent = data.GetComponent<FlavoringData>();

                return nodeComponent.ItemData;
            }
        }

        Debug.LogWarning($"UnlockID {unlockID}에 해당하는 데이터를 찾을 수 없습니다.");
        return null; // 유효하지 않은 NodeId 반환
    }
}