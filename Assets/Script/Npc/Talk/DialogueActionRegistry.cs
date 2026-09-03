using System.Collections.Generic;
using UnityEngine;

public class DialogueActionRegistry : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;

    private readonly Dictionary<string, IDialogueAction> actions = new();

    private void Awake()
    {
        // 자식 오브젝트에 붙은 모든 액션을 자동 수집 (비활성 포함)
        foreach (var action in GetComponentsInChildren<IDialogueAction>(true))
        {
            if (string.IsNullOrEmpty(action.ActionId))
            {
                Debug.LogWarning($"[ActionRegistry] actionId가 비어있는 액션: {(action as MonoBehaviour)?.name}");
                continue;
            }
            if (!actions.TryAdd(action.ActionId, action))
                Debug.LogError($"[ActionRegistry] 중복된 actionId: {action.ActionId}");
        }
    }

    // DialogueManager의 onActionRequested에 연결 (Dynamic string)
    public void OnActionRequested(string actionId)
    {
        if (!actions.TryGetValue(actionId, out var action))
        {
            Debug.LogError($"[ActionRegistry] 등록되지 않은 actionId: {actionId}. 대화가 멈추지 않도록 즉시 완료 처리합니다.");
            dialogueManager.NotifyActionComplete(actionId); // 안전장치
            return;
        }
        action.Execute(() => dialogueManager.NotifyActionComplete(actionId));
    }
}