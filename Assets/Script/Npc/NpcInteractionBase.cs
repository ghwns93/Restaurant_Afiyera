using UnityEngine;

public abstract class NpcInteractionBase : ScriptableObject, NpcInteractionInterface
{
    public string dialogueKey; // 대화 테이블 키

    public bool isUnRocked;

    public abstract void Execute(GameObject actor);
}
