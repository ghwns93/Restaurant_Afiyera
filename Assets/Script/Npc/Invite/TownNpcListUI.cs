using System.Collections.Generic;
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

    /// <summary>
    /// 지정한 마을의 NPC 목록을 UI로 갱신
    /// </summary>
    public void OpenUI(TownType currentTown)
    {
        // 1. 기존 슬롯 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 현재 마을의 전체 NPC 데이터 가져오기
        List<NpcSpecialEventData> townEvents = NpcSpecialEventManager.Instance.GetEventsByTown(currentTown);

        // 3. 슬롯 생성 및 해금 여부 적용
        foreach (var eventData in townEvents)
        {
            GameObject slotObj = Instantiate(npcSlotPrefab, contentParent);
            if (slotObj.TryGetComponent<TownNpcSlotUI>(out var slot))
            {
                bool isUnlocked = NpcSpecialEventManager.Instance.IsEventUnlocked(eventData.EventId);
                slot.Setup(eventData, isUnlocked);
            }
        }

        uiRoot.SetActive(true);
    }

    public void CloseUI()
    {
        uiRoot.SetActive(false);
    }
}