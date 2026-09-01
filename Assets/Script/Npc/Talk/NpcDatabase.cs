using System;
using System.Collections.Generic;
using UnityEngine;

// 프로젝트에 등장하는 모든 NPC의 표시 정보를 등록해두는 데이터베이스.
// Project 창에서 우클릭 → Create → Dialogue → Npc Database 로 생성.
[CreateAssetMenu(fileName = "NpcDatabase", menuName = "Dialogue/Npc Database")]

[Serializable]
public class NpcDatabase : ScriptableObject
{
    public string npcId;       // JSON의 speakerId / npcId와 매칭되는 키
    public string displayName; // 대화창 이름표에 표시할 이름
    public Sprite[] portraits;    // 초상화 스프라이트
}
