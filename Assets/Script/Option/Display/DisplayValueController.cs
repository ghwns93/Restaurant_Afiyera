using UnityEngine;

public class DisplayValueController : MonoBehaviour
{
    public static DisplayValueController Instance;

    //PlayerPrefs 에 저장하는 문자열 키
    public const string PREF_FULLSCREEN = "DISPLAY_SETTING_FULLSCREEN";
    public const string PREF_RES_X = "DISPLAY_SETTING_RES_X";
    public const string PREF_RES_Y = "DISPLAY_SETTING_RES_Y";
    public const string PREF_VSYNC = "DISPLAY_SETTING_VSYNC";
    public const string PREF_FPS_LIMIT = "DISPLAY_SETTING_FPS";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAndApplySettings();
    }

    // 저장된 값을 불러와서 적용하는 메서드
    private void LoadAndApplySettings()
    {
        // 기본값 설정 (저장된 데이터가 없을 경우를 대비)
        bool isFull = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;
        int resX = PlayerPrefs.GetInt(PREF_RES_X, 1920); // 사용자님의 FHD 타겟에 맞춰 1920 설정
        int resY = PlayerPrefs.GetInt(PREF_RES_Y, 1080);
        bool vsync = PlayerPrefs.GetInt(PREF_VSYNC, 0) == 1;
        int fps = PlayerPrefs.GetInt(PREF_FPS_LIMIT, 60);

        ApplySettings(isFull, resX, resY, vsync, fps);
    }

    // 실제 유니티 엔진에 설정을 반영하는 메서드
    private void ApplySettings(bool isFull, int x, int y, bool vsync, int fps)
    {
        // 1. 해상도 및 전체화면 설정
        Screen.SetResolution(x, y, isFull);

        // 2. VSync(수직 동기화) 설정 (0: 끔, 1: 켬)
        QualitySettings.vSyncCount = vsync ? 1 : 0;

        // 3. FPS 제한 설정
        // VSync가 켜져 있으면 targetFrameRate 설정보다 VSync가 우선순위를 가집니다.
        Application.targetFrameRate = fps;

        //Debug.Log($"설정 적용 완료: {x}x{y}, Full:{isFull}, VSync:{vsync}, FPS:{fps}");
    }

    public void SaveFullScreen(bool isFull)
    {
        PlayerPrefs.SetInt(PREF_FULLSCREEN, isFull ? 1 : 0);
    }

    public void SaveRes(int x, int y)
    {
        PlayerPrefs.SetInt(PREF_RES_X, x);
        PlayerPrefs.SetInt(PREF_RES_Y, y);
    }

    public void SaveVsync(bool vsync)
    {
        PlayerPrefs.SetInt(PREF_VSYNC, vsync ? 1 : 0);
    }

    public void SaveFPS(int fps)
    {
        PlayerPrefs.SetInt(PREF_FPS_LIMIT, fps);
    }
}
