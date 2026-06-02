using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcTalkUIManager : MonoBehaviour
{
    public static NpcTalkUIManager Instance;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    public TextMeshProUGUI npcTalkText;

    private List<NpcTalkButton> activeButtons = new List<NpcTalkButton>();
    private int selectedIndex = 0;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
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
            activeButtons[selectedIndex].Onclick();
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

    public void ShowSelectionButtons(List<NpcInteractionBase> dialogues, GameObject actor)
    {
        if(gameObject.activeSelf == false) gameObject.SetActive(true);

        SystemController.Instance.SetSystemPause(false);

        activeButtons.Clear();

        // 기존 버튼 제거
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);

        // 사용 가능한 대화만큼 버튼 생성
        foreach (var data in dialogues)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer).GetComponent<NpcTalkButton>();
            btn.Setup(data, actor);

            activeButtons.Add(btn);
        }
    }

    public void SetTalkText(string text)
    {
        if (gameObject.activeSelf == false) gameObject.SetActive(true);

        npcTalkText.text = text;
    }

    public void EndTalk()
    {
        npcTalkText.text = "";

        SystemController.Instance.SetSystemPause(true);

        gameObject.SetActive(false);
    }
}
