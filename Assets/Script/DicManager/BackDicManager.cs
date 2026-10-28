using UnityEngine;

public class BackDicManager : BaseDicManager<BackDicManager, string, DialogueBackInfo>
{
    protected override string GetKey(DialogueBackInfo data)
    {
        return data.Id.ToString().Trim();
    }
}
