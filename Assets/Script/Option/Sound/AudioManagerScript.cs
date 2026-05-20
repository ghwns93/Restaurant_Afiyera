using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;

public class AudioManagerScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private Slider masterSlider;        // MASTER 볼륨 슬라이더 (0~1)
    [SerializeField]
    private Slider bgmSlider;        // BGM 볼륨 슬라이더 (0~1)
    [SerializeField]
    private Slider sfxSlider;        // SFX 볼륨 슬라이더 (0~1)

    [SerializeField]
    private Text masterText;            // MASTER % 텍스트
    [SerializeField]
    private Text bgmText;            // BGM % 텍스트
    [SerializeField]
    private Text sfxText;            // SFX % 텍스트

    private float lastPlayTime = 0f;       // 미리듣기 연타 제한

    private bool isLoadComp = false;

    private void Start()
    {
        masterSlider.value = VolumeController.Instance.GetMasterVolume();
        bgmSlider.value = VolumeController.Instance.GetBGMVolume();
        sfxSlider.value = VolumeController.Instance.GetSFXVolume();

        isLoadComp = true;
    }

    private void OnEnable()
    {
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        bgmSlider.onValueChanged.RemoveListener(OnBgmChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    public void OnMasterChanged(float value)
    {
        masterText.text = VolumeController.Instance.ApplyMasterVolume(value).ToString().Trim();
    }

    public void OnBgmChanged(float value)
    {
        bgmText.text = VolumeController.Instance.ApplyBgmVolume(value).ToString().Trim();
    }

    public void OnSfxChanged(float value)
    {
        sfxText.text = VolumeController.Instance.ApplySfxVolume(value).ToString().Trim();

        if (Time.time - lastPlayTime > 0.1f && isLoadComp)
        {
            SoundManager.Instance.PlaySfxSound(SoundName.SFX_ClickDown);
            lastPlayTime = Time.time;
        }
    }
}
