using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // 인스펙터에서 등록할 사운드 데이터 리스트
    [SerializeField]
    private List<SoundData> soundDataList;

    // 실제 검색에 사용할 딕셔너리
    private Dictionary<SoundName, SoundData> soundDictionary = new Dictionary<SoundName, SoundData>();

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource sfxSource;

    void OnEnable()
    {
        // 씬이 로드될 때마다 버튼들을 새로 바인딩
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
                PlayBgmSound(SoundName.BGM_Farm);
                break;
            case "CookingGameScene":
                PlayBgmSound(SoundName.BGM_Misson);
                break;

        }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 초기화: 리스트를 딕셔너리로 변환 (인덱스 문제 해결)
        foreach (var data in soundDataList)
        {
            if (!soundDictionary.ContainsKey(data.soundName))
                soundDictionary.Add(data.soundName, data);
        }
    }

    public void PlayBgmSound(SoundName name)
    {
        if (soundDictionary.TryGetValue(name, out SoundData data))
        {
            // 믹서 그룹 설정 및 재생
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

    public void PlaySfxSound(SoundName name)
    {
        if (soundDictionary.TryGetValue(name, out SoundData data))
        {
            if (data.soundName != SoundName.None)
            {
                // 믹서 그룹 설정 및 재생
                sfxSource.outputAudioMixerGroup = data.group;
                sfxSource.pitch = data.pitch;
                sfxSource.PlayOneShot(data.clip, data.volume);
            }
        }
    }
}