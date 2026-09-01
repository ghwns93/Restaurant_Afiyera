using System;
using System.Collections.Generic;

// JSON 전체 구조. JsonUtility 호환을 위해 전부 [Serializable] + public 필드 사용.
// 선택지 분기를 위해 순차 배열이 아닌 "id 기반 그래프" 구조로 동작한다.
[Serializable]
public class DialogueData
{
    public string dialogueId;
    public string startLineId;      // 대화 시작 라인 id (비어있으면 lines[0]부터)
    public List<DialogueLine> lines;
}

// 대사 한 줄 (그래프의 노드)
[Serializable]
public class DialogueLine
{
    public string id;               // 라인 고유 id
    public string speakerId;        // NpcDatabase에 등록된 NPC id
    public string text;             // 리치 텍스트(<color> 등) 사용 가능
    public float typingSpeed = 0.04f; // 글자당 출력 간격(초). 작을수록 빠름
    public string next;             // 다음 라인 id. 비어있고 choices도 없으면 대화 종료
    public List<DialogueEvent> events;   // 이 라인 시작 시 실행할 이벤트
    public List<DialogueChoice> choices; // 비어있지 않으면 타이핑 완료 후 선택지 표시

    public bool HasChoices => choices != null && choices.Count > 0;
}

// 선택지 하나 (그래프의 간선)
[Serializable]
public class DialogueChoice
{
    public string text;             // 버튼에 표시할 텍스트
    public string next;             // 선택 시 이동할 라인 id. 비어있으면 대화 종료
    public List<DialogueEvent> events; // 선택 시 실행할 이벤트 (예: 거절하면 NPC 퇴장)
}

// 대사 라인/선택지에 붙는 이벤트 (NPC 중도 등장/퇴장 등)
[Serializable]
public class DialogueEvent
{
    public string type;   // "SHOW_NPC", "HIDE_NPC" (확장 가능: "PLAY_SFX", "CAMERA_SHAKE"...)
    public string npcId;  // 대상 NPC id
    public int slot;      // SHOW_NPC일 때 화면에 표시할 슬롯 번호
    public int portraitIndex; // SHOW_NPC일 때 표시할 초상화 인덱스
}

// 이벤트 타입 상수 (문자열 오타 방지용)
public static class DialogueEventType
{
    public const string ShowNpc = "SHOW_NPC";
    public const string HideNpc = "HIDE_NPC";
}
