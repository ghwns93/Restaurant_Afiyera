using UnityEngine;

public static class SFXPlayer
{
    // 1. 마우스 반응형 및 일반 2D 효과음
    public static void Play(SoundName name)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfxSound(name);
        }
    }

    // [추가] 낚시처럼 길게 재생되는 효과음 시작
    public static void PlayLong(SoundName name)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLongSfx(name);
        }
    }

    // [추가] 재생 중인 효과음 강제 중지 (취소 시)
    public static void Stop(SoundName name)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLongSfx(name);
        }
    }

    // 3. 상시형 오브젝트 사운드 등록 (분수 등)
    public static AudioSource RegisterAmbient(SoundName name, Vector3 position)
    {
        if (SoundManager.Instance != null)
        {
            return SoundManager.Instance.RegisterAmbient(name, position);
        }
        return null;
    }
}