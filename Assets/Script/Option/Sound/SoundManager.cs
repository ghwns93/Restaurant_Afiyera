using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private List<SoundData> soundDataList;
    private SoundData currentBgmData;

    [SerializeField] private List<VillageBgmInfo> villageBgmList;
    private Dictionary<SoundName, SoundData> soundDictionary = new Dictionary<SoundName, SoundData>();

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource; // UI 및 일반 2D 효과음용

    // [추가] 현재 재생 중인 길고 지속적인 SFX(낚시 등)를 관리하는 딕셔너리
    private Dictionary<SoundName, AudioSource> activeLongSfxSources = new Dictionary<SoundName, AudioSource>();

    // 상시형(Ambient) 사운드 오브젝트들을 관리하는 딕셔너리
    private Dictionary<SoundName, AudioSource> ambientSources = new Dictionary<SoundName, AudioSource>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "IntroScene":
                PlayBgmSound(SoundName.BGM_Title);
                break;
            case "VillageScene":
                PlayBgmSound(SoundName.BGM_IsiDora);
                break;
        }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var data in soundDataList)
        {
            if (!soundDictionary.ContainsKey(data.soundName))
                soundDictionary.Add(data.soundName, data);
        }
    }

    public void ChangeVillageBgm(TownType townType)
    {
        foreach (var villageBgm in villageBgmList)
        {
            if (villageBgm.towntype == townType)
            {
                PlayBgmSound(villageBgm.soundName);
                return;
            }
        }
    }

    public void PlayBgmSound(SoundName name)
    {
        if (soundDictionary.TryGetValue(name, out SoundData data))
        {
            if (currentBgmData == data) return;
            currentBgmData = data;

            bgmSource.outputAudioMixerGroup = data.group;
            bgmSource.pitch = data.pitch;
            bgmSource.loop = true;
            bgmSource.clip = data.clip;
            bgmSource.Play();
        }
    }

    public void StopBgmSound()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 2D 효과음 재생 (마우스 반응형, 상점 구매 등 위치가 필요 없는 경우)
    /// </summary>
    public void PlaySfxSound(SoundName name)
    {
        if (soundDictionary.TryGetValue(name, out SoundData data))
        {
            if (data.soundName != SoundName.None)
            {
                sfxSource.pitch = data.pitch;
                sfxSource.PlayOneShot(data.clip, data.volume);
            }
        }
    }

    // ==========================================
    // [추가] 도중에 멈출 수 있는 사운드 제어 기능 (낚시 등)
    // ==========================================

    /// <summary>
    /// 길게 재생되거나 도중 중지가 필요한 효과음 재생
    /// </summary>
    public void PlayLongSfx(SoundName name)
    {
        // 이미 재생 중이면 중복 실행 방지
        if (activeLongSfxSources.ContainsKey(name) && activeLongSfxSources[name] != null)
        {
            return;
        }

        if (soundDictionary.TryGetValue(name, out SoundData data))
        {
            if (data.soundName == SoundName.None) return;

            // 임시 오디오 소스 오브젝트 생성
            GameObject sfxObj = new GameObject($"LongSFX_{name}");
            AudioSource source = sfxObj.AddComponent<AudioSource>();

            source.outputAudioMixerGroup = data.group;
            source.clip = data.clip; 
            source.volume = data.volume;
            source.pitch = data.pitch; 
            source.loop = data.loop;
            source.Play();

            activeLongSfxSources.Add(name, source);

            // [핵심 추가] 루프가 아니거나(일반 사운드인데 PlayLong으로 틀었을 때), 
            // 혹은 끝까지 재생되었을 때를 감지하여 딕셔너리에서 자동 제거하는 코루틴 시작
            StartCoroutine(CheckAudioFinishedRoutine(name, source, sfxObj));
        }
    }

    /// <summary>
    /// 실행 중인 특정 효과음을 즉시 중지
    /// </summary>
    public void StopLongSfx(SoundName name)
    {
        if (activeLongSfxSources.TryGetValue(name, out AudioSource source))
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject); // 오브젝트 정리
            }
            activeLongSfxSources.Remove(name);
        }
    }

    /// <summary>
    /// 오디오 재생이 끝날 때까지 기다렸다가 딕셔너리에서 안전하게 지워주는 코루틴
    /// </summary>
    private IEnumerator CheckAudioFinishedRoutine(SoundName name, AudioSource source, GameObject sfxObj)
    {
        // loop가 켜져 있으면 사용자가 직접 StopLongSfx를 호출하기 전까지는 계속 대기
        // loop가 꺼져있는 클립이라면 끝날 때까지 대기
        if (!source.loop)
        {
            // 오디오 클립의 실제 재생 시간만큼 대기 (피치 변동 고려)
            float duration = source.clip.length / Mathf.Abs(source.pitch);
            yield return new WaitForSeconds(duration);
        }
        else
        {
            // 루프 사운드인 경우, 소스가 도중에 파괴되거나 멈출 때까지 대기
            while (source != null && source.isPlaying)
            {
                yield return null;
            }
        }

        // 재생이 끝났거나 소스가 꺼졌다면 딕셔너리 정리
        if (activeLongSfxSources.ContainsKey(name) && activeLongSfxSources[name] == source)
        {
            activeLongSfxSources.Remove(name);
        }

        // 오브젝트 정리
        if (sfxObj != null)
        {
            Destroy(sfxObj);
        }
    }

    /// <summary>
    /// 상시형(Ambient) 사운드 등록 (분수 소리 등 거리 감쇠 적용)
    /// </summary>
    public AudioSource RegisterAmbient(SoundName name, Vector3 position)
    {
        if (!soundDictionary.TryGetValue(name, out SoundData data)) return null;

        if (ambientSources.ContainsKey(name))
        {
            return ambientSources[name];
        }

        GameObject ambientObj = new GameObject($"Ambient_{name}");
        ambientObj.transform.position = position;

        AudioSource source = ambientObj.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = data.group;
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = true;

        // 3D 거리 감쇠 설정
        source.spatialBlend = 1.0f; // 완전한 3D 사운드
        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = data.maxHearingDistance;
        source.maxDistance = data.fadeDistance;
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, CreateDistanceCurve(data.maxHearingDistance, data.fadeDistance));

        source.Play();
        ambientSources.Add(name, source);

        return source;
    }

    private AnimationCurve CreateDistanceCurve(float maxHearingDist, float fadeDist)
    {
        AnimationCurve curve = new AnimationCurve();
        // 0 ~ maxHearingDist 까지는 볼륨 100% 유지
        curve.AddKey(new Keyframe(0f, 1f));
        curve.AddKey(new Keyframe(maxHearingDist, 1f));
        // maxHearingDist ~ fadeDist 까지 선형으로 볼륨 감소
        curve.AddKey(new Keyframe(fadeDist, 0f));
        return curve;
    }
}

[System.Serializable]
public struct VillageBgmInfo
{
    public TownType towntype;
    public SoundName soundName;
}