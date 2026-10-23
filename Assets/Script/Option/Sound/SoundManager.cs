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

            // 독립된 임시 오디오 소스 오브젝트 생성 (개별 제어를 위함)
            GameObject sfxObj = new GameObject($"LongSFX_{name}");
            AudioSource source = sfxObj.AddComponent<AudioSource>();

            source.outputAudioMixerGroup = data.group;
            source.clip = data.clip; 
            source.volume = data.volume; 
            source.pitch = data.pitch;
            source.loop = data.loop; // SoundData의 loop 설정 따름
            source.Play();

            activeLongSfxSources.Add(name, source);
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