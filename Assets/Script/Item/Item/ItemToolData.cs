using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    Axe,        // 도끼
    Pickaxe,    // 곡괭이
    Bucket,     // 양동이
    Knife,      // 칼
    Hand        // 맨손
}

public enum ResourceType
{
    Tree,       // 나무
    Rock,       // 바위
    Lake,       // 호수
    Animal      // 동물
}

[System.Serializable]
public struct DropItemInfo
{
    [SerializeField] private ItemData itemData;     // 획득할 재료 아이템
    [SerializeField] private int itemCount;         // 획득 수량
    [Range(0f, 100f)][SerializeField] private float dropChance;     // 확률 (0 ~ 100%)

    public ItemData ItemData => itemData;
    public int ItemCount => itemCount;
    public float DropChance => dropChance;
}

[CreateAssetMenu(fileName = "New Tool Item", menuName = "Inventory/Tool")]
public class ItemToolData : ItemData
{
    [SerializeField] private ToolType toolType;                         // 도구 종류
    [SerializeField] private ResourceType targetResourceType;           // 채집 가능한 자원 종류
    [SerializeField] private float gatherDuration = 3f;                 // 채집 시간

    [Header("Drop Table (도구 등급별 확률 설정)")]
    [SerializeField] private List<DropItemInfo> dropTable = new List<DropItemInfo>(); // 확률별 보상 리스트

    public ToolType ToolType => toolType;
    public ResourceType TargetResourceType => targetResourceType;
    public float GatherDuration => gatherDuration;
    public List<DropItemInfo> DropTable => dropTable;

    // 확률에 따라 아이템을 추첨하여 반환하는 메서드
    public (ItemData item, int count) GetRandomDropItem()
    {
        if (dropTable == null || dropTable.Count == 0) return (null, 0);

        // 1. 전체 확률의 가중치 합 계산
        float totalChance = 0f;
        foreach (var drop in dropTable)
        {
            totalChance += drop.DropChance;
        }

        // 2. 0부터 총합 사이의 랜덤값 추출
        float randomValue = Random.Range(0f, totalChance);
        float currentSum = 0f;

        // 3. 누적 확률을 이용해 당첨된 아이템 선정
        foreach (var drop in dropTable)
        {
            currentSum += drop.DropChance;
            if (randomValue <= currentSum)
            {
                return (drop.ItemData, drop.ItemCount);
            }
        }

        // 예외 상황 시 첫 번째 아이템 반환
        return (dropTable[0].ItemData, dropTable[0].ItemCount);
    }
}