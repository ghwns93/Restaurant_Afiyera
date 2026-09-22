using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcSpecialEventManager : MonoBehaviour
{
    public static NpcSpecialEventManager Instance { get; private set; }

    [Header("전체 마을 NPC 이벤트 Master Registry")]
    [SerializeField] private List<NpcSpecialEventData> allSpecialEvents;

    // 해금된 이벤트 ID 목록 보관
    private readonly HashSet<string> unlockedEventIds = new HashSet<string>();

    private string selectedEventId;

    public string SelectedEventId
    {
        get => selectedEventId;
        set => selectedEventId = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void UnlockEvent(NpcSpecialEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.EventId)) return;

        if (unlockedEventIds.Add(eventData.EventId))
        {
            Debug.Log($"[SpecialEventManager] 이벤트 해금:({eventData.EventId})");
        }
    }

    /// <summary>
    /// 특정 이벤트가 해금되었는지 여부
    /// </summary>
    public bool IsEventUnlocked(string eventId)
    {
        return unlockedEventIds.Contains(eventId);
    }

    /// <summary>
    /// 특정 마을 소속의 모든 NPC 이벤트 목록 가져오기
    /// </summary>
    public List<NpcSpecialEventData> GetEventsByTown(TownType town)
    {
        return allSpecialEvents.Where(e => e.Town == town).ToList();
    }

    public NpcSpecialEventData GetSelectedId()
    {
        return allSpecialEvents.FirstOrDefault(e => e.EventId == selectedEventId);
    }

    public void SetSelectedEventId(string eventId)
    {
        selectedEventId = eventId;
    }
}