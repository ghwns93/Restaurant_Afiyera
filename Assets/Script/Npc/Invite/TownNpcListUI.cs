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
                bool isConditionMet = action.CanActivate();
                bool isSeenByPlayer = NpcSpecialEventManager.Instance.IsEventSeen(action.name);

                if (!isCompleted)
                {
                    if (isConditionMet)
                    {
                        targetActionToShow = action;
                        // 조건이 충족되어 진행 가능해도, 플레이어가 직접 대화해봐야만 ? 가 풀림
                        isRevealedInUI = isSeenByPlayer;
                    }
                    else
                    {
                        targetActionToShow = action;
                        isRevealedInUI = false;
                    }
                    break; // 진행 대상(또는 조건에 막힌 최우선) 스토리를 결정했으므로 이 NPC 탐색 종료
                }
                else
                {
                    // 이미 완료된 과거 스토리는 저장 (모두 완료했을 때의 최종 상태 노출용)
                    targetActionToShow = action;
                    isRevealedInUI = true;
                }
            }

            // NPC당 추출된 최종 1개 액션으로 UI 슬롯 생성
            if (targetActionToShow != null)
            {
                GameObject slotObj = Instantiate(npcSlotPrefab, contentParent);
                if (slotObj.TryGetComponent<TownNpcSlotUI>(out var slot))
                {
                    slot.Setup(targetActionToShow.UnlockEventData, isRevealedInUI);
                }
            }
        }

        uiRoot.SetActive(true);
    }

    public void CloseUI()
    {
        uiRoot.SetActive(false);
    }
}