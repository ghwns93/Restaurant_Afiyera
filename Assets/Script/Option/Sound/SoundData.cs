using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSound", menuName = "Sound/SoundData")]
public class SoundData : ScriptableObject
{
    public SoundName soundName;     // 고유 키값[cite: 1]
    public AudioClip clip;          // 오디오 파일[cite: 1]
    public AudioMixerGroup group;   // 출력될 믹서 그룹 (BGM, SFX 등)[cite: 1]

    [Range(0f, 1.2f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;

    [Header("Ambient (상시형) Distance Settings")]
    public float maxHearingDistance = 5f;  // 최대 음량이 유지되는 거리
    public float fadeDistance = 25f;        // 소리가 완전히 사라지는 거리
}

[System.Serializable]
public enum SoundName
{
    None,
    BGM_Title,
    BGM_IsiDora,
    BGM_DesPina,
    BGM_ArMila,

    // 마우스 반응형
    SFX_CursorHover,
    SFX_ClickDown,
    SFX_ClickUp,
    SFX_EscClose,
    SFX_EscOpen,

    // 유저 반응형
    SFX_Walk_Wood,
    SFX_Walk_Rock,
    SFX_Walk_Sand,
    SFX_Walk_Ground,
    SFX_Gather_Insect,
    SFX_Gather_Insect_Last,
    SFX_Gather_Fishing,
    SFX_Gather_Mining,
    SFX_Gather_Hand,
    SFX_Gather_Magic,
    SFX_ShopPurchase,
    SFX_ToolSelect,
    SFX_MapChange_Ground,
    SFX_MapChange_Sand,
    SFX_MapChange_Wood,

    // 상시형
    SFX_TownFountain,

    // UI형
    SFX_Talk,
}