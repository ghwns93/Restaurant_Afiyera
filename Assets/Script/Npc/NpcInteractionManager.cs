using System.Collections.Generic;
using UnityEngine;

public class NpcInteractionManager : MonoBehaviour
{
    public static NpcInteractionManager Instance;

    // 퀘스트 ID와 완료 여부를 저장
    private Dictionary<NpcInteractionInterface, bool> questDatabase = new Dictionary<NpcInteractionInterface, bool>();

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void InputQuest(NpcInteractionInterface id, bool isCompleted)
    {
        if (!questDatabase.ContainsKey(id)) questDatabase.Add(id, isCompleted);
        else questDatabase[id] = isCompleted;
    }

    // 특정 이벤트/퀘스트 해금 (Talk 매니저 등이 호출)
    public void UnlockQuest(NpcInteractionInterface id)
    {
        if (!questDatabase.ContainsKey(id)) questDatabase.Add(id, true);
        else questDatabase[id] = true;
    }

    // 조건 충족 여부 확인
    public bool IsQuestCompleted(NpcInteractionInterface id)
    {
        return questDatabase.ContainsKey(id) && questDatabase[id];
    }
}
