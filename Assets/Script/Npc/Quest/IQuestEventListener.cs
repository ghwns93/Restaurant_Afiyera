using UnityEngine;

public interface IQuestEventListener
{
    // 퀘스트 상태 갱신 시 각 Interaction이 실행할 로직
    void OnEvaluateState(GameObject ownerNpc);
}