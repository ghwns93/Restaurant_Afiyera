using UnityEngine;

public class NpcDicManager : BaseDicManager<string, NpcDatabase>
{
    protected override string GetKey(NpcDatabase data)
    {
        return data.npcId;
    }
}
