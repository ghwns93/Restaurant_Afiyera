using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildableCountManager : MonoBehaviour
{
    public static BuildableCountManager Instance { get; private set; }

    private Dictionary<int, int> buildableCountDic = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBuildableCount(int buildID, int count)
    {
        if(buildableCountDic.ContainsKey(buildID))
        {
            buildableCountDic[buildID] += count;
        }
        else
        {
            buildableCountDic.Add(buildID, count);
        }
    }

    public void UseBuildableCount(int buildID, int count)
    {
        if(buildableCountDic.ContainsKey(buildID))
        {
            buildableCountDic[buildID] -= count;
            if(buildableCountDic[buildID] < 0)
            {
                buildableCountDic[buildID] = 0;
            }
        }
    }

    public int GetBuildableCount(int buildID)
    {
        if(buildableCountDic.TryGetValue(buildID, out int count))
        {
            return count;
        }
        else
        {
            return 0;
        }
    }

    public List<(int,int)> GetBuildableTotInfo()
    {
        return new List<(int key,int value)> (buildableCountDic.Select(kv => (kv.Key, kv.Value)));
    }
}
