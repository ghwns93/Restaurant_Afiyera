using UnityEngine;

public class NpcDicManager : BaseDicManager<NpcDicManager, string, NpcBasicInfo>
{
    protected override string GetKey(NpcBasicInfo data)
    {
        return data.npcId;
    }
}
