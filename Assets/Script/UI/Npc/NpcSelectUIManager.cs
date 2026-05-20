using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NpcSelectUIManager : MonoBehaviour
{
    public GameObject buttonPrefab; // 상호작용 버튼 프리팹
    public Transform container;     // 버튼들이 배치될 부모 객체

    private List<NpcSelectButton> activeButtons = new List<NpcSelectButton>();
    private int selectedIndex = 0;

    private void Awake()
    {
        NpcSelectEvents.OnNPCDetected += AddButton;
        NpcSelectEvents.OnNPCLost += RemoveButton;
    }

    private void Update()
    {
        if (activeButtons.Count == 0) return;

        // 1. 마우스 휠로 선택 이동
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            UpdateSelection(wheel > 0 ? -1 : 1);
        }

        // 2. F키로 확정 (선택된 버튼의 NPC 함수 호출)
        if (Input.GetKeyDown(KeyCode.F))
        {
            activeButtons[selectedIndex].ClickInteraction();
        }
    }

    private void AddButton(BasicNpcScript npc)
    {
        var go = Instantiate(buttonPrefab, container);
        var btn = go.GetComponent<NpcSelectButton>();
        btn.Setup(npc);
        activeButtons.Add(btn);
        RefreshVisuals();
    }

    private void RemoveButton(BasicNpcScript npc)
    {
        var btn = activeButtons.Find(b => b.TargetNPC == npc);
        if (btn != null)
        {
            activeButtons.Remove(btn);
            Destroy(btn.gameObject);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, activeButtons.Count - 1);
            RefreshVisuals();
        }
    }

    private void UpdateSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + activeButtons.Count) % activeButtons.Count;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        for (int i = 0; i < activeButtons.Count; i++)
        {
            activeButtons[i].SetHighlight(i == selectedIndex);
        }
    }
}