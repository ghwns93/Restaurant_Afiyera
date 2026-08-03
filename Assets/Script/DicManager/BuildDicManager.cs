using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildDicManager : BaseDicManager<int, GameObject>
{
    protected override int GetKey(GameObject data)
    {
        var nodeComponent = data.GetComponent<BasicNode>();
        if (nodeComponent != null)
        {
            return nodeComponent.NodeId;
        }
        else
        {
            Debug.LogWarning($"GameObject {data.name}에 BasicNode 컴포넌트가 없습니다.");
            return -1; // 유효하지 않은 키 반환
        }
    }

    public List<GameObject> GetTypeValue(Type bs)
    {
        List<GameObject> resultList = new List<GameObject>();
        foreach (var data in dataList)
        {
            var nodeComponent = data.GetComponent<BasicNode>();
            if (nodeComponent != null && nodeComponent.GetType() == bs)
            {
                resultList.Add(data);
            }
        }
        return resultList;
    }

    public int GetNodeIdByUnlockID(string unlockID)
    {
        foreach (var data in dataList)
        {
            var unlockComponent = data.GetComponent<ShopUnlockableItem>();

            if (unlockComponent != null && unlockComponent.UnlockID == unlockID)
            {
                var nodeComponent = data.GetComponent<BasicNode>();

                return nodeComponent.NodeId;
            }
        }

        Debug.LogWarning($"UnlockID {unlockID}에 해당하는 NodeId를 찾을 수 없습니다.");
        return -1; // 유효하지 않은 NodeId 반환
    }
}
