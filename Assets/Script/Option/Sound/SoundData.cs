using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSound", menuName = "Sound/SoundData")]
public class SoundData : ScriptableObject
{
    public SoundName soundName;     // 고유 키값
    public AudioClip clip;          // 오디오 파일
    public AudioMixerGroup group;   // 출력될 믹서 그룹 (BGM, SFX 등)

    [Range(0f, 1.2f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
}

[System.Serializable]
public enum SoundName
{
    None,
    BGM_Title,
    BGM_IsiDora,
    BGM_DesPina,
    BGM_ArMila,
    SFX_ClickDown,
    SFX_ClickUp,
    SFX_CursorHover,
}