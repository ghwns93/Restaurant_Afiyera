using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGatherController : MonoBehaviour
{
    [SerializeField] private KeyCode gatherKey = KeyCode.F; // 채집 매핑 키

    private ResourceObject currentTargetResource = null;
    private Coroutine gatherCoroutine = null;
    private bool isGathering = false;

    private void Start()
    {
        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback += OnEquippedItemChanged;
        }
    }

    private void OnDestroy()
    {
        if (PlayerInventoryController.Instance != null)
        {
            PlayerInventoryController.Instance.onSelectedSlotChangedCallback -= OnEquippedItemChanged;
        }
    }

    private void Update()
    {
        HandleGatherInput();
    }

    public void EnterResourceRange(ResourceObject resource)
    {
        currentTargetResource = resource;
        EvaluateGatherCondition();
    }

    public void ExitResourceRange(ResourceObject resource)
    {
        if (currentTargetResource == resource)
        {
            StopGathering();
            currentTargetResource = null;

            if (GatherUIController.Instance != null)
            {
                GatherUIController.Instance.HideGatherUI();
            }
        }
    }

    private void OnEquippedItemChanged(int slotIndex)
    {
        if (isGathering)
        {
            StopGathering();
        }
        EvaluateGatherCondition();
    }

    private void EvaluateGatherCondition()
    {
        if (currentTargetResource == null)
        {
            if (GatherUIController.Instance != null) GatherUIController.Instance.HideGatherUI();
            return;
        }

        if (CheckCanGather())
        {
            if (GatherUIController.Instance != null && !isGathering)
            {
                GatherUIController.Instance.ShowGatherUI(gatherKey, "채집");
            }
        }
        else
        {
            if (GatherUIController.Instance != null && !isGathering)
            {
                GatherUIController.Instance.HideGatherUI();
            }
        }
    }

    private bool CheckCanGather()
    {
        if (currentTargetResource == null) return false;

        ItemData equippedItem = PlayerInventoryController.Instance != null ? PlayerInventoryController.Instance.GetCurrentlyEquippedItem() : null;

        if (equippedItem is ItemToolData toolData)
        {
            if (toolData.TargetResourceType == currentTargetResource.ResourceType)
            {
                // 도구가 요구하는 미끼 조건도 추가로 확인할 수 있습니다.
                return true;
            }
        }
        return false;
    }

    private void HandleGatherInput()
    {
        if (currentTargetResource == null) return;

        if (Input.GetKeyDown(gatherKey) && !isGathering)
        {
            if (CheckCanGather())
            {
                ItemToolData toolData = PlayerInventoryController.Instance.GetCurrentlyEquippedItem() as ItemToolData;
                float duration = toolData != null ? toolData.GatherDuration : 3f;

                gatherCoroutine = StartCoroutine(GatherProcessRoutine(duration, toolData));
            }
        }

        if (isGathering && Input.GetKeyUp(gatherKey))
        {
            Debug.Log("채집 키를 떼어 취소되었습니다.");
            StopGathering();
        }
    }

    private IEnumerator GatherProcessRoutine(float duration, ItemToolData toolData)
    {
        isGathering = true;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            if (GatherUIController.Instance != null)
            {
                GatherUIController.Instance.SetProgress(progress);
            }

            yield return null;
        }

        CompleteGather(toolData);
    }

    private void StopGathering()
    {
        if (gatherCoroutine != null)
        {
            StopCoroutine(gatherCoroutine);
            gatherCoroutine = null;
        }
        isGathering = false;

        if (GatherUIController.Instance != null)
        {
            GatherUIController.Instance.SetProgress(0f);
        }
        EvaluateGatherCondition();
    }

    private void CompleteGather(ItemToolData toolData)
    {
        isGathering = false;
        gatherCoroutine = null;

        if (currentTargetResource == null) return;

        // 1. 현재 장착/선택된 미끼 가져오기
        ItemBaitData currentBait = PlayerInventoryController.Instance != null ? PlayerInventoryController.Instance.GetCurrentlyEquippedBait(toolData.ToolType) : null;

        if (currentBait == null)
        {
            ItemBaitData defaultBait = FindAlternativeBait(toolData); // 앞서 만든 도구 전용 미끼 탐색 함수 활용

            if (defaultBait != null)
            {
                // 자동으로 찾은 미끼를 장착 상태로 등록
                PlayerInventoryController.Instance.EquipBaitForTool(toolData.ToolType, defaultBait);
                currentBait = defaultBait;
                Debug.Log($"장착된 미끼가 없어 인벤토리에서 [{defaultBait.ItemName}]을(를) 자동으로 장착했습니다.");
            }
            else
            {
                // 인벤토리에도 미끼가 전혀 없는 경우
                Debug.Log("미끼가 장착되어 있지 않습니다.");
                if (GatherUIController.Instance != null) GatherUIController.Instance.SetProgress(0f);
                EvaluateGatherCondition();
                return;
            }
        }

        // 2. 도구가 허용하는 최대 미끼 등급 검사
        if (toolData != null && currentBait.BaitTier > toolData.MaxAllowedBaitTier)
        {
            Debug.Log("이 도구는 해당 미끼를 사용할 수 없습니다! (도구 등급 부족)");
            if (GatherUIController.Instance != null) GatherUIController.Instance.SetProgress(0f);
            EvaluateGatherCondition();
            return;
        }

        // 3. [핵심 변경] 자원 오브젝트에게 미끼를 전달하여 해당 자원의 드롭 테이블에서 아이템 추첨
        var dropped = currentTargetResource.GetDropItemByBait(currentBait);
        ItemData rewardItem = dropped.item;
        int rewardCount = dropped.count;

        if (rewardItem != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItem, rewardCount);
            Debug.Log($"채집 성공! [{rewardItem.ItemName} x{rewardCount}] 획득 완료 (사용된 미끼: {currentBait.ItemName})");
        }

        // 4. 미끼 소모 처리 (무제한이 아닐 경우 1개 차감)
        if (!currentBait.IsInfinite && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ReduceItem(currentBait, 1);

            // 만약 소모 후 해당 미끼의 남은 개수가 0개라면?
            if (InventoryManager.Instance.GetItemCount(currentBait) <= 0)
            {
                Debug.Log($"[{currentBait.ItemName}] 미끼를 모두 소모했습니다. 대체 미끼를 찾습니다.");

                // 현재 도구에서 사용 가능한 다른 미끼를 인벤토리에서 검색하여 자동 장착
                ItemBaitData nextAvailableBait = FindAlternativeBait(toolData);
                PlayerInventoryController.Instance.EquipBaitForTool(toolData.ToolType, nextAvailableBait); // 없으면 null이 들어가서 해제됨
            }
        }

        if (GatherUIController.Instance != null)
        {
            GatherUIController.Instance.SetProgress(0f);
        }
        EvaluateGatherCondition();
    }

    // 현재 도구에서 사용할 수 있고, 개수가 남아있는 다른 미끼를 찾는 함수
    private ItemBaitData FindAlternativeBait(ItemToolData toolData)
    {
        if (InventoryManager.Instance == null || toolData == null) return null;

        // 인벤토리에서 미끼 슬롯들을 가져옴 (프로젝트 구조에 맞게 메서드명 수정 필요)
        List<ItemSlot> allBaitSlots = InventoryManager.Instance.GetSlots(ItemType.Bait);

        foreach (var slot in allBaitSlots)
        {
            if (slot.ItemData is ItemBaitData baitData)
            {
                // 1. 도구 타입이 일치하고
                // 2. 도구가 허용하는 최대 미끼 등급 이하이며
                // 3. 인벤토리에 수량이 남아있는 경우 (무제한이거나 개수가 0 초과)
                bool hasEnoughCount = baitData.IsInfinite || slot.Quantity > 0; // 혹은 slot.Quantity 등 프로젝트 필드명 사용

                if (baitData.TargetToolType == toolData.ToolType &&
                    baitData.BaitTier <= toolData.MaxAllowedBaitTier &&
                    hasEnoughCount)
                {
                    // 현재 소모된 미끼와 다른 미끼를 우선적으로 찾거나, 발견한 첫 번째 미끼 반환
                    return baitData;
                }
            }
        }

        // 사용 가능한 대체 미끼가 없으면 null 반환 (미끼 해제)
        return null;
    }
}