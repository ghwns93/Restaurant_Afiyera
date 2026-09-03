using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// 대화씬 전체를 제어하는 매니저.
// - JSON 로드 → id 기반 그래프 탐색으로 라인 재생 (선택지 분기 지원)
// - 라인/선택지 이벤트 처리 (NPC 중도 등장/퇴장)
// - TMP maxVisibleCharacters 기반 타자 효과 (리치 텍스트 안전)
// - 클릭 1회: 타이핑 즉시 완성 / 2회: 다음 라인
// - 오토 모드: 타이핑 완료 후 일정 시간 뒤 자동 진행 (선택지에서 정지)
// - 스킵 모드: Ctrl 홀드 또는 토글로 빠르게 넘기기 (선택지에서 정지)
public class DialogueManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject dialoguePanel;   // 대화창 전체 루트
    [SerializeField] private TMP_Text nameText;          // 화자 이름
    [SerializeField] private TMP_Text bodyText;          // 대사 본문 (TextMeshPro)
    [SerializeField] private GameObject nextIndicator;   // "▼" 같은 다음 표시 아이콘
    [SerializeField] private NpcSlotView[] npcSlots;     // 화면에 배치된 NPC 슬롯들
    [SerializeField] private TempCookAndNightCookScene nightCookScene;

    [Header("선택지 UI")]
    [SerializeField] private Transform choiceContainer;          // 버튼들이 배치될 부모 (Vertical Layout Group 권장)
    [SerializeField] private DialogueChoiceButton choiceButtonPrefab; // 선택지 버튼 프리팹

    [Header("타자 효과")]
    [SerializeField] private float defaultTypingSpeed = 0.04f; // JSON에 값이 없을 때 사용
    [SerializeField] private float punctuationPause = 0.25f;   // 문장부호 뒤 추가 딜레이
    [SerializeField] private AudioSource typingSfx;            // 글자 출력음 (선택)
    [SerializeField] private int sfxEveryNChars = 2;           // N글자마다 효과음 1회

    [Header("오토 모드")]
    [SerializeField] private float autoBaseDelay = 1.0f;       // 오토 진행 기본 대기 시간
    [SerializeField] private float autoDelayPerChar = 0.03f;   // 글자 수에 비례해 추가되는 대기 시간 (긴 문장 = 더 오래 표시)

    [Header("모드 변경 알림 (UI 버튼 하이라이트용)")]
    public UnityEvent<bool> onAutoModeChanged;

    private DialogueData currentDialogue;
    private Dictionary<string, DialogueLine> lineMap; // id → 라인 빠른 조회
    private DialogueLine currentLine;
    private bool isTyping;
    private bool choicesVisible;
    private bool dialogueActive;
    private Coroutine typingRoutine;
    private Coroutine autoRoutine;
    private readonly List<DialogueChoiceButton> spawnedButtons = new List<DialogueChoiceButton>();

    // 모드 상태
    private bool autoMode;

    public bool IsDialogueActive => dialogueActive;
    public bool IsAutoMode => autoMode;

    [Header("액션 이벤트")]
    public UnityEvent<string> onActionRequested; // actionId를 받아 실제 연출을 시작하는 곳

    private bool waitingForAction;
    private string pendingActionId;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        if (nextIndicator != null) nextIndicator.SetActive(false);
        if (choiceContainer != null) choiceContainer.gameObject.SetActive(false);

        StartDialogueFromResources("dialogue_intro");
    }

    private void Update()
    {
        if (!dialogueActive) return;

        // 선택지 표시 중엔 스킵/오토/진행 입력 전부 정지. 버튼으로만 진행
        if (choicesVisible || waitingForAction) return;

        // 마우스 클릭 / 스페이스 / 엔터로 진행
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            // 수동 조작 시 오토 모드 해제 (유지하고 싶으면 이 줄 제거)
            if (autoMode) SetAutoMode(false);
            OnAdvanceInput();
        }
    }

    // ===== 모드 제어 (UI 버튼의 onClick에 연결) =====

    public void ToggleAutoMode() => SetAutoMode(!autoMode);

    public void SetAutoMode(bool on)
    {
        if (autoMode == on) return;
        autoMode = on;
        onAutoModeChanged?.Invoke(on);

        if (on)
        {
            // 이미 타이핑이 끝나 대기 중인 상태에서 켰다면 즉시 오토 카운트 시작
            if (dialogueActive && !isTyping && !choicesVisible)
                StartAutoAdvance();
        }
        else
        {
            CancelAutoAdvance();
        }
    }

    // ===== 오토 =====

    private void StartAutoAdvance()
    {
        CancelAutoAdvance();
        autoRoutine = StartCoroutine(AutoAdvanceRoutine());
    }

    private void CancelAutoAdvance()
    {
        if (autoRoutine != null)
        {
            StopCoroutine(autoRoutine);
            autoRoutine = null;
        }
    }

    private IEnumerator AutoAdvanceRoutine()
    {
        // 긴 문장일수록 읽을 시간을 더 준다
        int charCount = bodyText.textInfo.characterCount;
        float wait = autoBaseDelay + charCount * autoDelayPerChar;
        yield return new WaitForSeconds(wait);

        autoRoutine = null;
        if (dialogueActive && !choicesVisible && !isTyping)
            JumpToLine(currentLine.next);
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
        CancelAutoAdvance(); // 라인 전환 시 남아있는 오토 카운트 초기화

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
        ProcessLineEvents(line);

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
                    int portraitIndex = ev.portraitIndex < npc.portraits.Length ? ev.portraitIndex : 0;
                    npcSlots[ev.slot].Show(ev.npcId, npc != null ? npc.portraits[portraitIndex] : null);
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

    // 라인의 이벤트 처리: ACTION_PLAY는 예약(pendingActionId), 나머지는 즉시 실행
    // PlayCurrentLine과 SkipToChoiceOrEnd 양쪽 공용
    private void ProcessLineEvents(DialogueLine line)
    {
        pendingActionId = null;
        if (line.events != null)
        {
            foreach (var ev in line.events)
            {
                if (ev.type == DialogueEventType.ActionPlay)
                    pendingActionId = ev.actionId;
                else
                    HandleEvent(ev);
            }
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

        // 선택지가 있으면 "▼" 대신 선택지 버튼 표시 (오토/스킵도 여기서 정지)
        if (currentLine.HasChoices)
        {
            ShowChoices(currentLine.choices);
            return;
        }

        // ▼ 추가: 예약된 액션이 있으면 발동하고 대기
        if (!string.IsNullOrEmpty(pendingActionId))
        {
            waitingForAction = true;
            if (nextIndicator != null) nextIndicator.SetActive(false);
            onActionRequested?.Invoke(pendingActionId);
            return;  // 오토 카운트도 시작하지 않음
        }

        if (nextIndicator != null) nextIndicator.SetActive(true);

        // 오토 모드면 자동 진행 카운트 시작 (스킵 중엔 TickSkip이 진행을 담당)
        if (autoMode)
            StartAutoAdvance();
    }

    // 액션 완료 시 외부 스크립트가 호출. actionId가 일치해야 재개됨
    public void NotifyActionComplete(string actionId)
    {
        if (!waitingForAction || pendingActionId != actionId) return;

        waitingForAction = false;
        pendingActionId = null;
        JumpToLine(currentLine.next);  // 자동으로 다음 대사 진행
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
        waitingForAction = false;
        pendingActionId = null;
        CancelAutoAdvance();
        SetAutoMode(false);   // 오토도 해제 (유지하고 싶으면 이 줄 제거)
        if (choicesVisible) HideChoices();
        dialoguePanel.SetActive(false);

        foreach (var slot in npcSlots)
            if (slot.IsOccupied) slot.Hide();

        // 필요하면 여기서 onDialogueEnd 이벤트 발행 (퀘스트 시작, 플레이어 조작 복구 등)

        nightCookScene.NightCookEnd();
    }

    // ===== 스킵: 가장 가까운 선택지(없으면 마지막 라인)까지 즉시 점프 =====
    // 스킵 버튼의 onClick에 연결
    public void SkipToChoiceOrEnd()
    {
        if (!dialogueActive || choicesVisible || waitingForAction) return;

        // 진행 중인 타이핑/오토 정리
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = null;
        CancelAutoAdvance();
        isTyping = false;

        // 그래프를 따라 전진. 선택지 라인을 만나거나 next가 없으면 정지
        var visited = new HashSet<string> { currentLine.id }; // 순환 참조 안전장치
        DialogueLine line = currentLine;

        while (!line.HasChoices && !HasActionEvent(line) && !string.IsNullOrEmpty(line.next))
        {
            if (!lineMap.TryGetValue(line.next, out DialogueLine nextLine))
            {
                Debug.LogError($"[DialogueManager] 존재하지 않는 라인 id: {line.next}. 대화를 종료합니다.");
                EndDialogue();
                return;
            }
            if (!visited.Add(nextLine.id)) break; // 무한 루프 감지 시 중단

            line = nextLine;

            // 건너뛰는 라인의 이벤트도 전부 실행 → NPC 등장/퇴장 상태가 정상적으로 반영됨
            ProcessLineEvents(line);
        }

        currentLine = line;
        ShowLineInstantly(line);
    }

    // 타자 효과 없이 라인을 즉시 완성 상태로 표시
    private void ShowLineInstantly(DialogueLine line)
    {
        var npc = NpcDicManager.Instance.GetData(line.speakerId);
        nameText.text = npc != null ? npc.displayName : line.speakerId;

        foreach (var slot in npcSlots)
            slot.SetSpeaking(slot.IsOccupied && slot.CurrentNpcId == line.speakerId);

        bodyText.text = line.text;
        bodyText.maxVisibleCharacters = int.MaxValue;
        bodyText.ForceMeshUpdate();

        FinishTyping(); // 선택지 라인이면 버튼 표시, 아니면 ▼ 표시
    }

    // 헬퍼 추가
    private bool HasActionEvent(DialogueLine line)
    {
        if (line.events == null) return false;
        foreach (var ev in line.events)
            if (ev.type == DialogueEventType.ActionPlay) return true;
        return false;
    }
}
