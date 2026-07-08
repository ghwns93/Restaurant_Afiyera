using System.Collections.Generic;
using UnityEngine;

public class NpcInteractionManager : MonoBehaviour
{
    public static NpcInteractionManager Instance;

    // 퀘스트 ID와 완료 여부를 저장
    private Dictionary<NpcInteractionInterface, QuestState> questComplete = new Dictionary<NpcInteractionInterface, QuestState>();

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteQuest(NpcInteractionInterface id)
    {
        if (!questComplete.ContainsKey(id))
        {
            //QuestState nextState = (id.resetType == QuestSO.ResetType.Permanent)
            //? QuestState.PermanentlyLocked
            //: QuestState.Completed;
        }
    }

    // 조건 충족 여부 확인
    public bool IsQuestCompleted(NpcInteractionInterface id)
    {
        if (questComplete.TryGetValue(id, out QuestState state))
        {
            // Completed 상태이거나 영구 락 상태이면 완료된 것으로 판단
            return state == QuestState.Completed || state == QuestState.PermanentlyLocked;
        }
        return false;
    }
}

public enum QuestState
{
    NotStarted,  // 시작 안 함 (진행 가능)
    Completed,   // 완료됨 (리셋 대상)
    PermanentlyLocked // 완료됨 (리셋 대상 아님)
}
