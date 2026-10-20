using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GardenObjectDicManager : BaseDicManager<GardenObjectDicManager, int, GardenObjectData>
{
    protected override int GetKey(GardenObjectData data)
    {
        return data.Id;
    }
}