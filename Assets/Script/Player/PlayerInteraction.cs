using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactionRange = 1.5f; // 상호작용 거리
    [SerializeField] private LayerMask npcLayer; // NPC 레이어
    [SerializeField] private int MaxNPCCount = 3; // 최대 감지 NPC 수

    // 현재 감지 범위 안에 있는 NPC들을 관리하는 리스트
    private List<BasicNpcScript> nearNPCs = new List<BasicNpcScript>();

    private bool canTalk = true;

    private void OnEnable()
    {
        // 이벤트 구독
        SystemController.OnSystemStateChanged += HandleSystemState;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 해제 필수!
        SystemController.OnSystemStateChanged -= HandleSystemState;
    }

    private void HandleSystemState(bool isPaused)
    {
        // 신호가 올 때만 이 함수가 실행됩니다.
        canTalk = isPaused;

        if(canTalk == false)
        {
            // NPC 리스트 초기화 및 UI 제거 이벤트 발생
            foreach (var npc in nearNPCs)
            {
                NpcSelectEvents.OnNPCLost?.Invoke(npc);
            }
            nearNPCs.Clear();
        }
    }

    void Update()
    {
        if(canTalk) CheckForNPCs();
    }

    private void CheckForNPCs()
    {
        // 설정한 반경 내의 모든 NPC 콜라이더
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, npcLayer);

        List<BasicNpcScript> currentFoundNPCs = hitColliders
        .Select(col => col.GetComponent<BasicNpcScript>()) // 콜라이더에서 NPCController 추출
        .Where(npc => npc != null)                        // Null 제외
        .OrderBy(npc => Vector2.Distance(transform.position, npc.transform.position)) // 거리순 정렬
        .Take(MaxNPCCount)                                          // 상위 5개만 선택
        .ToList();

        // UI 생성 이벤트
        foreach (var npc in currentFoundNPCs)
        {
            if (!nearNPCs.Contains(npc))
            {
                nearNPCs.Add(npc);
                NpcSelectEvents.OnNPCDetected?.Invoke(npc);
            }
        }

        // UI 제거 이벤트
        for (int i = nearNPCs.Count - 1; i >= 0; i--)
        {
            if (!currentFoundNPCs.Contains(nearNPCs[i]))
            {
                BasicNpcScript lostNPC = nearNPCs[i];
                nearNPCs.RemoveAt(i);
                NpcSelectEvents.OnNPCLost?.Invoke(lostNPC);
            }
        }
    }
}