using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public int id;
    public Vector3Int position;
    public int remainHarvestTime = -1; // 남은 수확 시간
}

[Serializable]
public class QuestData
{
    public string id;
    public QuestType isCompleted;
}

[Serializable]
public class SaveData
{
    public int day;

    public List<BuildingData> buildings = new List<BuildingData>();
    public List<QuestData> quests = new List<QuestData>();
}
