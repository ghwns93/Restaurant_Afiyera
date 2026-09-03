using System;
using UnityEngine;

public interface IDialogueAction
{
    string ActionId { get; }
    void Execute(Action onComplete); // 완료 시 onComplete() 호출
}

// 인스펙터에서 actionId를 지정할 수 있는 MonoBehaviour 베이스
public abstract class DialogueActionBase : MonoBehaviour, IDialogueAction
{
    [SerializeField] private string actionId;
    public string ActionId => actionId;

    public abstract void Execute(Action onComplete);
}