using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecialEvent", menuName = "Quest/Special Event Data")]
public class NpcSpecialEventData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string eventId;        // 이벤트 고유 ID
    [SerializeField] private string eventName;      // 이벤트 이름
    [SerializeField] private TownType town;         // 이벤트가 속한 마을
    [SerializeField] private NpcBasicInfo npcInfo;  // 이벤트 npc 정보
    [SerializeField] private int storyOrder;         // 이벤트 순서

    [Header("UI Display Info")]
    [SerializeField] private List<ItemIngredientData> requiredIngredients; // 이벤트 해금에 필요한 재료 목록

    [Header("Dialogue Data (JSON)")]
    [SerializeField] private TextAsset dialogueJsonFile;

    public string EventId => eventId;
    public string EventName => eventName;
    public int StoryOrder => storyOrder;
    public TownType Town => town;
    public NpcBasicInfo NpcInfo => npcInfo;
    public List<ItemIngredientData> RequiredIngredients => requiredIngredients;
    public string DialogueJsonText => dialogueJsonFile != null ? dialogueJsonFile.text : string.Empty;
}