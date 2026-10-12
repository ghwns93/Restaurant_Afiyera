using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ResourceDropItem
{
    [SerializeField] private ItemData itemData;     // 획득할 아이템
    [SerializeField] private int itemCount;         // 획득 수량

    public ItemData ItemData => itemData;
    public int ItemCount => itemCount;
}

public class ResourceObject : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;

    [Header("이 자원의 고유 드롭 테이블 (마을마다 다르게 설정 가능)")]
    [Header("인덱스 0 = 슬롯 0, 인덱스 1 = 슬롯 1, 인덱스 2 = 슬롯 2 ...")]
    [SerializeField] private List<ResourceDropItem> resourceDropTable = new List<ResourceDropItem>();

    public ResourceType ResourceType => resourceType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResourceGatherController gatherController = other.GetComponent<ResourceGatherController>();
            if (gatherController != null)
            {
                gatherController.EnterResourceRange(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResourceGatherController gatherController = other.GetComponent<ResourceGatherController>();
            if (gatherController != null)
            {
                gatherController.ExitResourceRange(this);
            }
        }
    }

    // 미끼의 가중치 정보를 받아 이 자원에서 어떤 아이템이 나올지 추첨하는 메서드
    public (ItemData item, int count) GetDropItemByBait(ItemBaitData baitData)
    {
        if (resourceDropTable == null || resourceDropTable.Count == 0) return (null, 0);

        // 미끼 정보가 없거나 가중치 설정이 안 되어있다면 기본적으로 0번 슬롯 반환
        if (baitData == null || baitData.SlotWeights == null || baitData.SlotWeights.Count == 0)
        {
            return (resourceDropTable[0].ItemData, resourceDropTable[0].ItemCount);
        }

        // 1. 미끼에 설정된 가중치들의 총합 계산
        float totalWeight = 0f;
        foreach (var sw in baitData.SlotWeights)
        {
            // 자원이 가진 드롭 테이블 범위를 벗어나는 인덱스는 제외
            if (sw.SlotIndex >= 0 && sw.SlotIndex < resourceDropTable.Count)
            {
                totalWeight += sw.Weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return (resourceDropTable[0].ItemData, resourceDropTable[0].ItemCount);
        }

        // 2. 랜덤값 추출
        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        // 3. 누적 가중치로 당첨된 슬롯 결정
        foreach (var sw in baitData.SlotWeights)
        {
            if (sw.SlotIndex < 0 || sw.SlotIndex >= resourceDropTable.Count) continue;

            currentSum += sw.Weight;
            if (randomValue <= currentSum)
            {
                var dropInfo = resourceDropTable[sw.SlotIndex];
                return (dropInfo.ItemData, dropInfo.ItemCount);
            }
        }

        // 예외 상황 시 첫 번째 슬롯 반환
        return (resourceDropTable[0].ItemData, resourceDropTable[0].ItemCount);
    }
}