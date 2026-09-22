using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecialEvent", menuName = "Quest/Special Event Data")]
public class NpcSpecialEventData : ScriptableObject
{
    [Header("Event Identification")]
    [SerializeField] private string eventId; 
    [SerializeField] private Sprite npcSprite;
    [SerializeField] private TownType town; // 어느 마을 소속인지 지정

    [Header("UI Display Info")]
    [SerializeField] private List<ItemIngredientData> requiredIngredients; // 이벤트 해금에 필요한 재료 목록

    [Header("Dialogue Data (JSON)")]
    [SerializeField] private TextAsset dialogueJsonFile;

    public string EventId => eventId;
    public Sprite NpcSprite => npcSprite;
    public TownType Town => town;
    public List<ItemIngredientData> RequiredIngredients => requiredIngredients;
    public string DialogueJsonText => dialogueJsonFile != null ? dialogueJsonFile.text : string.Empty;
}

public enum TownType
{
    VillageA,
    VillageB,
    VillageC
}