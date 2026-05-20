using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class VolumeController : MonoBehaviour
{
    public static VolumeController Instance;

    [SerializeField]
    [Range(0f, 20f)]
    private float soundMaxValue;

    [Header("Mixer Settings")]
    [SerializeField]
    private AudioMixer audioMixer;   // 오디오 믹서
    [SerializeField]
    private string masterParam = "Master"; // Exposed parameter 이름
    [SerializeField]
    private string bgmParam = "BGMVolume";
    [SerializeField]
    private string sfxParam = "SFXVolume";

    //PlayerPrefs 에 저장하는 문자열 키
    public const string PREF_MASTER = "VOLUME_MASTER";
    public const string PREF_BGM = "VOLUME_BGM";
    public const string PREF_SFX = "VOLUME_SFX";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 저장된 값 불러오기
        float masterValue = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        float bgmValue = PlayerPrefs.GetFloat(PREF_BGM, 1f);
        float sfxValue = PlayerPrefs.GetFloat(PREF_SFX, 1f);

        ApplyMasterVolume(masterValue);
        ApplyBgmVolume(bgmValue);
        ApplySfxVolume(sfxValue);
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(PREF_MASTER, 1f); ;
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat(PREF_BGM, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(PREF_SFX, 1f);
    }

    public float ApplyMasterVolume(float linear)
    {
        audioMixer.SetFloat(masterParam, CalculLinear(linear));

        float debug = 0f;

        audioMixer.GetFloat(masterParam, out debug);

        PlayerPrefs.SetFloat(PREF_MASTER, linear);

        return Mathf.RoundToInt(linear + 80.0f);
    }

    public float ApplyBgmVolume(float linear)
    {
        audioMixer.SetFloat(bgmParam, CalculLinear(linear));

        PlayerPrefs.SetFloat(PREF_BGM, linear);

        return Mathf.RoundToInt(linear + 80.0f);
    }

    public float ApplySfxVolume(float linear)
    {
        audioMixer.SetFloat(sfxParam, CalculLinear(linear));

        PlayerPrefs.SetFloat(PREF_SFX, linear);

        return Mathf.RoundToInt(linear + 80.0f);
    }

    private float CalculLinear(float linear)
    {
        float result = Mathf.Lerp(-80f, soundMaxValue, (linear + 80.0f) / 100f);

        return result;
    }
}
