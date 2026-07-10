using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public int id;
    public Vector3Int position;
}

[Serializable]
public class SaveData
{
    public int day;

    public List<BuildingData> buildings = new List<BuildingData>();
}
