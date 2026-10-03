using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcSpecialEventManager : MonoBehaviour
{
    public static NpcSpecialEventManager Instance { get; private set; }

    [Header("전체 Special Event Unlock Action Registry")]
    [SerializeField] private List<SpecialEventUnlockAction> allSpecialActions;

    // 플레이어가 직접 대화하여 확인한 액션 ID (UI ? 해제용)
    private readonly HashSet<string> seenActionIds = new HashSet<string>();

    private NpcSpecialEventData SelectedEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 대화 시작 또는 완료 시점에 호출하여 UI의 ? 표시 해제
    /// </summary>
    public void MarkEventAsSeen(string actionName)
    {
        if (!string.IsNullOrEmpty(actionName))
        {
            seenActionIds.Add(actionName);
        }
    }

    /// <summary>
    /// 플레이어가 직접 대화를 확인했는지 여부
    /// </summary>
    public bool IsEventSeen(string actionName)
    {
        return seenActionIds.Contains(actionName);
    }

    /// <summary>
    /// 특정 마을 소속의 SpecialEventUnlockAction 목록만 추출
    /// </summary>
    public List<SpecialEventUnlockAction> GetSpecialActionsByTown(TownType town)
    {
        return allSpecialActions.Where(a => a != null && a.UnlockEventData.Town == town).ToList();
    }

    public void SetSelectedEventId(NpcSpecialEventData selectedEvent)
    {
        SelectedEvent = selectedEvent;
    }

    public NpcSpecialEventData GetSelectedId()
    {
        return SelectedEvent;
    }

    public void ResetEvent()
    {
        SelectedEvent = null;
    }
}