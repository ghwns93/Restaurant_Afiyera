using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Npc", menuName = "Npc/NpcBasicInfo")]
public class NpcBasicInfo : ScriptableObject
{
    [Header("Basic Info")]
    public int npcCode = 0;
    public string npcName = "상인";
    [Tooltip("전신 이미지")]
    public Sprite npcImage;
    [Tooltip("아이콘 이미지")]
    public Sprite npcIconImage;
    [Tooltip("SD 이미지")]
    public Sprite npcSdImage;

    public string npcId => string.Format("npc.{0}.{1}", npcCode, npcName);

    [Header("Night Restaurant Info")]
    public bool isNightRestaurantNpc = false;                                               // 야간 식당 NPC 여부
    [ShowIf("isNightRestaurantNpc")] public Sprite[] portraits;                             // 초상화 스프라이트

    [Header("Interaction Info")]
    public bool isInteractionNpc = false; // 상호작용 NPC 여부
    [ShowIf("isInteractionNpc")] public NpcInteractionBase npcInteractionBase; // NPC가 가진 상호작용 정보 (예: 대화, 퀘스트 등)
    [ShowIf("isInteractionNpc")] public List<NpcInteractionBase> npcInteractionList;
    [ShowIf("isInteractionNpc")] public NpcType npcType;
    [ShowIf("npcType", NpcType.Shop)] public List<ShopItemData> shopItemList = new List<ShopItemData>(); // 상점에서 판매하는 아이템 리스트
    [ShowIf("npcType", NpcType.Shop)] public List<BuyIngredientData> ingredientItemList = new List<BuyIngredientData>(); // 상점에서 구매하는 아이템 리스트
}

public enum TownType
{
    IsiDora,
    DesPina,
    ArMila
}

public enum NpcType
{
    Npc,
    Shop
}