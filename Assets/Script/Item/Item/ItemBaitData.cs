using System.Collections.Generic;
using UnityEngine;

public enum BaitTier
{
    Basic = 1,      // 일반 미끼 (무제한)
    Advanced = 2,   // 고급 미끼 (소모성)
    Master = 3      // 최상급 미끼 (소모성)
}

[System.Serializable]
public struct BaitSlotWeight
{
    [SerializeField] private int slotIndex;             // 자원 드롭 테이블의 슬롯 인덱스 (0번, 1번, 2번...)
    [Range(0f, 100f)][SerializeField] private float weight; // 해당 슬롯의 가중치 또는 확률 비율

    public int SlotIndex => slotIndex;
    public float Weight => weight;
}

[CreateAssetMenu(fileName = "New Bait Item", menuName = "Inventory/Bait")]
public class ItemBaitData : ItemData
{
    [SerializeField] private ToolType targetToolType;
    [SerializeField] private BaitTier baitTier;                                         // 미끼 등급
    [SerializeField] private bool isInfinite = false;                                   // 무제한 여부
    [Header("미끼별 자원 슬롯 가중치 (0번, 1번, 2번 슬롯 확률 설정)")]
    [SerializeField] private List<BaitSlotWeight> slotWeights = new List<BaitSlotWeight>();

    public ToolType TargetToolType => targetToolType;
    public BaitTier BaitTier => baitTier;
    public bool IsInfinite => isInfinite;
    public List<BaitSlotWeight> SlotWeights => slotWeights;
}