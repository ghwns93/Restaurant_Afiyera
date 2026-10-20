using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    Pickaxe,            // 곡괭이
    FishingRod,         // 낚싯대
    Magic,              // 마법
    Hand,               // 맨손
    DragonFlyNet,       // 잠자리채
}

public enum ResourceType
{
    Rock,              // 바위
    Water,             // 물
    Mistic,            // 신비한 자원
    Plant,             // 식물
    Insect,            // 곤충
    Shop,              // 상점구매
}

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory/Tool")]
public class ItemToolData : ItemData
{
    [SerializeField] private ToolType toolType;                     // 도구 종류
    [SerializeField] private ResourceType targetResourceType;       // 채집 가능한 자원 종류
    [SerializeField] private float gatherDuration = 3f;             // 채집 시간
    [SerializeField] private BaitTier maxAllowedBaitTier = BaitTier.Basic; // 사용 가능한 최대 미끼 등급

    public ToolType ToolType => toolType;
    public ResourceType TargetResourceType => targetResourceType;
    public float GatherDuration => gatherDuration;
    public BaitTier MaxAllowedBaitTier => maxAllowedBaitTier;
}