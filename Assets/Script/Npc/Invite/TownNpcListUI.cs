using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TownNpcListUI : MonoBehaviour
{
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject npcSlotPrefab;

    private static TownNpcListUI _instance;

    public static TownNpcListUI Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        CloseUI();
    }

    public void OpenUI(TownType currentTown)
    {
        // 1. 기존 UI 슬롯 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 현재 마을 소속의 모든 SpecialEventUnlockAction 가져오기
        List<SpecialEventUnlockAction> townActions = NpcSpecialEventManager.Instance.GetSpecialActionsByTown(currentTown);

        // 3. NpcBasicInfo 기준 그룹화 및 스토리 순서 정렬
        var actionsGroupedByNpc = townActions
            .OrderBy(a => a.UnlockEventData.StoryOrder)
            .GroupBy(a => a.UnlockEventData.NpcInfo);

        // 4. 각 NPC별로 표기할 단 1개의 액션/스토리 추출
        foreach (var npcGroup in actionsGroupedByNpc)
        {
            if (npcGroup.Key == null) continue;

            SpecialEventUnlockAction targetActionToShow = null;
            bool isRevealedInUI = false;

            foreach (var action in npcGroup)
            {
                // 완료 여부는 NpcInteractionManager에서 체크
                bool isCompleted = NpcInteractionManager.Instance.IsQuestCompleted(action.UnlockEventData.NpcInfo.npcId, action);
                //bool isConditionMet = action.CanActivate();
                bool isSeenByPlayer = NpcSpecialEventManager.Instance.IsEventSeen(action.UnlockEventData.name);

                if (isSeenByPlayer) continue;

                // 조건이 충족되어 진행 가능해도, 플레이어가 직접 대화해봐야만 ? 가 풀림
                targetActionToShow = action;
                isRevealedInUI = isCompleted;

                break; // 진행 대상(또는 조건에 막힌 최우선) 스토리를 결정했으므로 이 NPC 탐색 종료
            }

            if (targetActionToShow == null) continue; // 모든 스토리가 이미 완료되었거나, 플레이어가 이미 본 스토리라면 UI에 표시하지 않음

            GameObject slotObj = Instantiate(npcSlotPrefab, contentParent);
            if (slotObj.TryGetComponent<TownNpcSlotUI>(out var slot))
            {
                slot.Setup(targetActionToShow.UnlockEventData, isRevealedInUI);
            }
        }

        uiRoot.SetActive(true);
    }

    public void CloseUI()
    {
        SystemController.Instance.SetSystemPause(true);
        uiRoot.SetActive(false);
    }
}