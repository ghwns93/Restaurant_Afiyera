using UnityEngine;

public class NpcDicManager : BaseDicManager<NpcDicManager, string, NpcDatabase>
{
    protected override string GetKey(NpcDatabase data)
    {
        return data.npcId;
    }
}
