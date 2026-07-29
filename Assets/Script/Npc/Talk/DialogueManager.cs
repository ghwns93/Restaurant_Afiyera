using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 대화씬 전체를 제어하는 매니저.
// - JSON 로드 → id 기반 그래프 탐색으로 라인 재생 (선택지 분기 지원)
// - 라인/선택지 이벤트 처리 (NPC 중도 등장/퇴장)
// - TMP maxVisibleCharacters 기반 타자 효과 (리치 텍스트 안전)
// - 클릭 1회: 타이핑 즉시 완성 / 2회: 다음 라인
// - 선택지가 있는 라인은 타이핑 완료 후 버튼 표시, 선택 전까지 진행 입력 차단
public class DialogueManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject dialoguePanel;   // 대화창 전체 루트
    [SerializeField] private TMP_Text nameText;          // 화자 이름
    [SerializeField] private TMP_Text bodyText;          // 대사 본문 (TextMeshPro)
    [SerializeField] private GameObject nextIndicator;   // "▼" 같은 다음 표시 아이콘
    [SerializeField] private NpcSlotView[] npcSlots;     // 화면에 배치된 NPC 슬롯들

    [Header("선택지 UI")]
    [SerializeField] private Transform choiceContainer;          // 버튼들이 배치될 부모 (Vertical Layout Group 권장)
    [SerializeField] private DialogueChoiceButton choiceButtonPrefab; // 선택지 버튼 프리팹

    [Header("타자 효과")]
    [SerializeField] private float defaultTypingSpeed = 0.04f; // JSON에 값이 없을 때 사용
    [SerializeField] private float punctuationPause = 0.25f;   // 문장부호 뒤 추가 딜레이
    [SerializeField] private AudioSource typingSfx;            // 글자 출력음 (선택)
    [SerializeField] private int sfxEveryNChars = 2;           // N글자마다 효과음 1회

    private DialogueData currentDialogue;
    private Dictionary<string, DialogueLine> lineMap; // id → 라인 빠른 조회
    private DialogueLine currentLine;
    private bool isTyping;
    private bool choicesVisible;
    private bool dialogueActive;
    private Coroutine typingRoutine;
    private readonly List<DialogueChoiceButton> spawnedButtons = new List<DialogueChoiceButton>();

    public bool IsDialogueActive => dialogueActive;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        if (nextIndicator != null) nextIndicator.SetActive(false);
        if (choiceContainer != null) choiceContainer.gameObject.SetActive(false);

        //임시
        StartDialogueFromResources("dialogue_intro");
    }

    private void Update()
    {
        if (!dialogueActive) return;
        if (choicesVisible) return; // 선택지 표시 중엔 버튼으로만 진행

        // 마우스 클릭 / 스페이스 / 엔터로 진행
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnAdvanceInput();
        }
    }

    // ===== 외부에서 대화 시작 =====

    // Resources/Dialogues/파일명.json 로드 후 시작
    public void StartDialogueFromResources(string fileNameWithoutExt)
    {
        TextAsset json = Resources.Load<TextAsset>($"Dialogues/{fileNameWithoutExt}");
        if (json == null)
        {
            Debug.LogError($"[DialogueManager] Resources/Dialogues/{fileNameWithoutExt}.json 을 찾을 수 없습니다.");
            return;
        }
        StartDialogue(json.text);
    }

    // JSON 문자열로 직접 시작 (파일, 서버, StreamingAssets 등 어디서 오든 사용 가능)
    public void StartDialogue(string jsonText)
    {
        currentDialogue = JsonUtility.FromJson<DialogueData>(jsonText);
        if (currentDialogue == null || currentDialogue.lines == null || currentDialogue.lines.Count == 0)
        {
            Debug.LogError("[DialogueManager] JSON 파싱 실패 또는 대사가 비어 있습니다.");
            return;
        }

        // id → 라인 맵 구성
        lineMap = new Dictionary<string, DialogueLine>();
        foreach (var line in currentDialogue.lines)
        {
            if (string.IsNullOrEmpty(line.id))
            {
                Debug.LogError("[DialogueManager] id가 비어있는 라인이 있습니다. 모든 라인에 고유 id가 필요합니다.");
                return;
            }
            if (lineMap.ContainsKey(line.id))
            {
                Debug.LogError($"[DialogueManager] 중복된 라인 id: {line.id}");
                return;
            }
            lineMap.Add(line.id, line);
        }

        dialogueActive = true;
        dialoguePanel.SetActive(true);

        // 이전 대화의 NPC가 남아있으면 정리
        foreach (var slot in npcSlots)
            if (slot.IsOccupied) slot.Hide();

        string startId = string.IsNullOrEmpty(currentDialogue.startLineId)
            ? currentDialogue.lines[0].id
            : currentDialogue.startLineId;

        JumpToLine(startId);
    }

    // ===== 진행 =====

    private void OnAdvanceInput()
    {
        if (isTyping)
        {
            // 1차 클릭: 타이핑 스킵하고 전체 문장 즉시 표시
            CompleteTypingImmediately();
        }
        else
        {
            // 2차 클릭: 다음 라인으로 (선택지 라인은 여기 도달하지 않음 → choicesVisible로 차단됨)
            JumpToLine(currentLine.next);
        }
    }

    // id로 라인 이동. 비어있거나 없는 id면 대화 종료
    private void JumpToLine(string lineId)
    {
        if (string.IsNullOrEmpty(lineId))
        {
            EndDialogue();
            return;
        }
        if (!lineMap.TryGetValue(lineId, out DialogueLine line))
        {
            Debug.LogError($"[DialogueManager] 존재하지 않는 라인 id: {lineId}. 대화를 종료합니다.");
            EndDialogue();
            return;
        }

        currentLine = line;
        PlayCurrentLine();
    }

    private void PlayCurrentLine()
    {
        DialogueLine line = currentLine;

        // 1) 라인에 붙은 이벤트 먼저 실행 (등장 → 대사 순서가 자연스러움)
        if (line.events != null)
            foreach (var ev in line.events)
                HandleEvent(ev);

        // 2) 화자 이름 표시 + 말하는 NPC 강조
        var npc = NpcDicManager.Instance.GetData(line.speakerId);
        nameText.text = npc != null ? npc.displayName : line.speakerId;

        foreach (var slot in npcSlots)
            slot.SetSpeaking(slot.IsOccupied && slot.CurrentNpcId == line.speakerId);

        // 3) 타자 효과 시작
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeLine(line));
    }

    private void HandleEvent(DialogueEvent ev)
    {
        switch (ev.type)
        {
            case DialogueEventType.ShowNpc:
            {
                if (ev.slot < 0 || ev.slot >= npcSlots.Length)
                {
                    Debug.LogWarning($"[DialogueManager] 잘못된 슬롯 번호: {ev.slot}");
                    return;
                }
                var npc = NpcDicManager.Instance.GetData(ev.npcId);
                npcSlots[ev.slot].Show(ev.npcId, npc != null ? npc.portrait : null);
                break;
            }
            case DialogueEventType.HideNpc:
            {
                foreach (var slot in npcSlots)
                    if (slot.CurrentNpcId == ev.npcId)
                        slot.Hide();
                break;
            }
            default:
                Debug.LogWarning($"[DialogueManager] 알 수 없는 이벤트 타입: {ev.type}");
                break;
        }
    }

    // ===== 타자 효과 =====
    // 핵심: 문자열을 잘라 넣는 대신 전체 텍스트를 넣고 maxVisibleCharacters를 올린다.
    // 이 방식은 <color> 같은 리치 텍스트 태그가 타이핑 중간에 깨지지 않고,
    // 레이아웃도 미리 계산되어 줄바꿈이 튀지 않는다.
    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        if (nextIndicator != null) nextIndicator.SetActive(false);

        bodyText.text = line.text;
        bodyText.maxVisibleCharacters = 0;
        bodyText.ForceMeshUpdate(); // 파싱된 실제 글자 수 확보

        int totalChars = bodyText.textInfo.characterCount; // 태그 제외한 보이는 글자 수
        float speed = line.typingSpeed > 0f ? line.typingSpeed : defaultTypingSpeed;

        for (int i = 0; i < totalChars; i++)
        {
            bodyText.maxVisibleCharacters = i + 1;

            // 효과음
            if (typingSfx != null && sfxEveryNChars > 0 && i % sfxEveryNChars == 0)
                typingSfx.PlayOneShot(typingSfx.clip);

            // 방금 출력한 글자 확인 → 문장부호면 잠깐 멈춰서 말하는 리듬 표현
            char c = bodyText.textInfo.characterInfo[i].character;
            float delay = speed;
            if (c == '.' || c == ',' || c == '!' || c == '?' || c == '…')
                delay += punctuationPause;

            yield return new WaitForSeconds(delay);
        }

        FinishTyping();
    }

    private void CompleteTypingImmediately()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        bodyText.maxVisibleCharacters = int.MaxValue;
        FinishTyping();
    }

    private void FinishTyping()
    {
        isTyping = false;
        typingRoutine = null;

        // 선택지가 있으면 "▼" 대신 선택지 버튼 표시
        if (currentLine.HasChoices)
        {
            ShowChoices(currentLine.choices);
        }
        else
        {
            if (nextIndicator != null) nextIndicator.SetActive(true);
        }
    }

    // ===== 선택지 =====

    private void ShowChoices(List<DialogueChoice> choices)
    {
        choicesVisible = true;
        choiceContainer.gameObject.SetActive(true);

        foreach (var choice in choices)
        {
            DialogueChoiceButton btn = Instantiate(choiceButtonPrefab, choiceContainer);
            spawnedButtons.Add(btn);

            // 클로저 캡처 주의: 지역 변수로 복사
            DialogueChoice captured = choice;
            btn.Setup(captured.text, () => OnChoiceSelected(captured));
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        HideChoices();

        // 선택지에 붙은 이벤트 실행 (예: 거절 시 NPC 퇴장)
        if (choice.events != null)
            foreach (var ev in choice.events)
                HandleEvent(ev);

        JumpToLine(choice.next);
    }

    private void HideChoices()
    {
        choicesVisible = false;
        foreach (var btn in spawnedButtons)
            if (btn != null) Destroy(btn.gameObject);
        spawnedButtons.Clear();
        choiceContainer.gameObject.SetActive(false);
    }

    // ===== 종료 =====

    private void EndDialogue()
    {
        dialogueActive = false;
        if (choicesVisible) HideChoices();
        dialoguePanel.SetActive(false);

        foreach (var slot in npcSlots)
            if (slot.IsOccupied) slot.Hide();

        // 필요하면 여기서 onDialogueEnd 이벤트 발행 (퀘스트 시작, 플레이어 조작 복구 등)
    }
}
