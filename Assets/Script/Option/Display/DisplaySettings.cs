using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [SerializeField]
    private ResolutionOption[] resolutionOptions;

    [SerializeField]
    private int[] fpsOptions;

    [SerializeField]
    private int defaultFPS;

    [SerializeField]
    private Toggle fullscreenToggle;

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    [SerializeField]
    private Toggle vSynscToggle;

    [SerializeField]
    private TMP_Dropdown fpsDropdown;

    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        // 초기 설정값 적용
        fullscreenToggle.isOn = Screen.fullScreen;

        vSynscToggle.isOn = QualitySettings.vSyncCount > 0;

        fpsDropdown.options.Clear();
        foreach (var option in fpsOptions)
        {
            fpsDropdown.options.Add(new TMP_Dropdown.OptionData($"{option} fps"));
        }
        fpsDropdown.RefreshShownValue();
        fpsDropdown.value = GetFPSIndex(Application.targetFrameRate);

        qualityDropdown.value = QualitySettings.GetQualityLevel();

        // 해상도 옵션 초기화
        resolutionDropdown.options.Clear();
        foreach (var option in resolutionOptions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{option.width} x {option.height}"));
        }
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.value = GetResolutionIndex(Screen.width, Screen.height);
    }

    private int GetFPSIndex(int targetFrameRate)
    {
        for (int i = 0; i < fpsOptions.Length; i++)
        {
            if (fpsOptions[i] == targetFrameRate)
                return i;
        }

        for(int i = 0; i < fpsOptions.Length; i++)
        {
            if (fpsOptions[i] == defaultFPS)
                return i;
        }

        return 0; 
    }

    private int GetResolutionIndex(int width, int height)
    {
        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            if (resolutionOptions[i].width == width && resolutionOptions[i].height == height)
                return i;
        }
        return 0;
    }

    public void openSettingPanel()
    {
        gameObject.SetActive(true);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        DisplayValueController.Instance.SaveFullScreen(isFullscreen);
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutionOptions[index].width, resolutionOptions[index].height, Screen.fullScreen);

        DisplayValueController.Instance.SaveRes(resolutionOptions[index].width, resolutionOptions[index].height);
    }

    public void SetVSync(bool enable)
    {
        QualitySettings.vSyncCount = enable ? 1 : 0;

        DisplayValueController.Instance.SaveVsync(enable);
    }

    public void SetFPSLimit(int fpsIndex)
    {
        int fps = defaultFPS;

        if (fpsIndex >= 0 && fpsIndex < fpsOptions.Length)
        {
            fps = fpsOptions[fpsIndex];
        }

        Application.targetFrameRate = fps;

        DisplayValueController.Instance.SaveFPS(fps);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}

[System.Serializable]
public struct ResolutionOption
{
    public int width;
    public int height;
}
