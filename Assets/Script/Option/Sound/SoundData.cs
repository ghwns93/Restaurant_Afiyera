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
    BGM_Misson,
    BGM_Arbiter,
    BGM_Farm,
    BGM_Cook,
    BGM_Title,
    SFX_BellDone,
    SFX_CatFeed1,
    SFX_CatFeed2,
    SFX_CatFeed3,
    SFX_CatFeed4,
    SFX_CatFeed5,
    SFX_CatInstall1,
    SFX_CatInstall2,
    SFX_CatInstall3,
    SFX_CatInstall4,
    SFX_CatInstall5,
    SFX_ClickDown,
    SFX_ClickUp,
    SFX_CowFeed1,
    SFX_CowFeed2,
    SFX_CowFeed3,
    SFX_CowInstall1,
    SFX_CowInstall2,
    SFX_CowInstall3,
    SFX_CursorHover,
    SFX_MissionAccept,
    SFX_MissionFail,
    SFX_MissionSuccess,
    SFX_NewOrder,
    SFX_PaperFlip1,
    SFX_PaperFlip2,
    SFX_PaperFlip3,
    SFX_PaperFlip4,
}