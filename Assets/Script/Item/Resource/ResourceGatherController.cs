using System.Collections;
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
            StopGathering(); // 도구를 바꾸면 채집 취소 및 프로그레스바 초기화
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
                return true;
            }
        }
        return false;
    }

    private void HandleGatherInput()
    {
        if (currentTargetResource == null) return;

        // 채집 시작 (KeyDown)
        if (Input.GetKeyDown(gatherKey) && !isGathering)
        {
            if (CheckCanGather())
            {
                ItemToolData toolData = PlayerInventoryController.Instance.GetCurrentlyEquippedItem() as ItemToolData;
                float duration = toolData != null ? toolData.GatherDuration : 3f;

                gatherCoroutine = StartCoroutine(GatherProcessRoutine(duration, toolData));
            }
        }

        // 채집 중 키를 떼었을 때 취소 (KeyUp)
        if (isGathering && Input.GetKeyUp(gatherKey))
        {
            Debug.Log("채집 키를 떼어 취소되었습니다.");
            StopGathering();
        }
    }

    // 채집 진행 및 프로그레스바 갱신 코루틴
    private IEnumerator GatherProcessRoutine(float duration, ItemToolData toolData)
    {
        isGathering = true;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration; // 0.0 ~ 1.0 비율 계산

            // UI 프로그레스바에 현재 진행률 전달
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

        // 취소되면 프로그레스바를 다시 0으로 비우고 상태 재확인
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

        ItemData rewardItem = null;
        int rewardCount = 0;

        if (toolData != null && toolData.DropTable != null && toolData.DropTable.Count > 0)
        {
            var dropped = toolData.GetRandomDropItem();
            rewardItem = dropped.item;
            rewardCount = dropped.count;
        }
        else if (currentTargetResource.RewardItemData != null)
        {
            rewardItem = currentTargetResource.RewardItemData;
            rewardCount = currentTargetResource.RewardItemCount;
        }

        if (rewardItem != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItem, rewardCount);
            Debug.Log($"채집 성공! [{rewardItem.ItemName} x{rewardCount}] 획득 완료.");
        }

        // 완료 후 프로그레스바 초기화 및 UI 재갱신
        if (GatherUIController.Instance != null)
        {
            GatherUIController.Instance.SetProgress(0f);
        }
        EvaluateGatherCondition();
    }
}