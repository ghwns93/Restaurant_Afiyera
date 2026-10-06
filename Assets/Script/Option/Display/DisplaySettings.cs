using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Header("해상도 관련 옵션")]
    [SerializeField] private Transform resolutionPanelContent;      // 패널 내부의 Content (Vertical Layout Group 등이 있는 위치)
    [SerializeField] private GameObject resolutionNodePrefab;       // ResolutionNodes가 부착된 프리팹
    [SerializeField] private ResolutionOption[] resolutionOptions; 
    [SerializeField] private int maxResolutionCount = 5;            // 최대 해상도 옵션 개수 (고화질부터 저화질까지 골고루 5개 선택)

    private List<ResolutionNodes> spawnedNodes = new List<ResolutionNodes>();
    private int currentResoulutionSelectedIndex = -1;

    [Header("FPS 관련 옵션")]
    [SerializeField] private Transform fpsPanelContent;      // FPS 패널 내부의 Content
    [SerializeField] private GameObject fpsNodePrefab;       // FpsNodes가 부착된 프리팹
    [SerializeField] private int[] fpsOptions;               // 지원할 FPS 배열 (예: 144, 60, 30 등)
    [SerializeField] private int maxFpsCount = 5;            // 원하는 FPS 옵션 최대 개수

    private List<FpsNodes> spawnedFpsNodes = new List<FpsNodes>();
    private int currentFpsSelectedIndex = -1;

    [SerializeField]
    private int defaultFPS;

    [SerializeField]
    private Toggle fullscreenToggle;

    [SerializeField]
    private Toggle vSynscToggle;

    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        // 초기 설정값 적용
        fullscreenToggle.isOn = Screen.fullScreen;

        vSynscToggle.isOn = QualitySettings.vSyncCount > 0;

        qualityDropdown.value = QualitySettings.GetQualityLevel();

        InitializeDisplayOptions();

        // 해상도 옵션 초기화
        CreateResolutionUI();

        // FPS 옵션 초기화
        CreateFpsUI();
    }

    private void InitializeDisplayOptions()
    {
        // 1. 1920x1080 비율 계산 (16:9)
        float targetAspectRatio = 1920f / 1080f;
        float tolerance = 0.01f;

        Resolution[] availableResolutions = Screen.resolutions;
        List<ResolutionOption> validResolutions = new List<ResolutionOption>();

        // 면적이 큰 순서(고해상도 -> 저해상도)로 정렬
        System.Array.Sort(availableResolutions, (a, b) => (b.width * b.height).CompareTo(a.width * a.height));

        foreach (var res in availableResolutions)
        {
            float ratio = (float)res.width / res.height;

            // 16:9 비율과 일치하는지 확인
            if (Mathf.Abs(ratio - targetAspectRatio) <= tolerance)
            {
                ResolutionOption option = new ResolutionOption
                {
                    width = res.width,
                    height = res.height
                };

                // 중복 해상도 제거
                bool isDuplicate = false;
                foreach (var existing in validResolutions)
                {
                    if (existing.width == option.width && existing.height == option.height)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    validResolutions.Add(option);
                }
            }
        }

        // 2. 고화질부터 저화질까지 골고루 5개 채우기
        List<ResolutionOption> selectedResolutions = new List<ResolutionOption>();
        int takeCount = Mathf.Min(maxResolutionCount, validResolutions.Count);

        if (validResolutions.Count <= maxResolutionCount)
        {
            // 지원하는 16:9 해상도가 maxResolutionCount개 이하면 전부 사용
            selectedResolutions = validResolutions;
        }
        else
        {
            // maxResolutionCount개 초과인 경우, 고화질부터 저화질 사이에서 간격을 두고 maxResolutionCount개 선택
            // (예: 제일 높은 것, 중간중간 골고루, 제일 낮은 것 형태)
            for (int i = 0; i < maxResolutionCount; i++)
            {
                // 0부터 validResolutions.Count - 1 사이를 maxResolutionCount등분하여 골고루 인덱스 추출
                // 만약 개수가 1개뿐일 때의 예외 처리 (ZeroDivision 방지)
                int index = (takeCount == 1) ? 0 : Mathf.RoundToInt((float)i / (takeCount - 1) * (validResolutions.Count - 1));
                selectedResolutions.Add(validResolutions[index]);
            }

            // 내림차순(고해상도 -> 저해상도) 순서가 유지되도록 정렬
            selectedResolutions.Sort((a, b) => (b.width * b.height).CompareTo(a.width * a.height));
        }

        resolutionOptions = selectedResolutions.ToArray();

        // 2. 주사율(FPS) 목록 수집 및 정렬
        HashSet<int> uniqueFpsSet = new HashSet<int>();
        foreach (var res in availableResolutions)
        {
            int hz = Mathf.RoundToInt((float)res.refreshRateRatio.value);
            if (hz > 0)
            {
                uniqueFpsSet.Add(hz);
            }
        }

        List<int> sortedFpsList = new List<int>(uniqueFpsSet);
        // 높은 주사율부터 낮은 주사율 순으로 내림차순 정렬 (예: 240, 144, 75, 60, 30 등)
        sortedFpsList.Sort((a, b) => b.CompareTo(a));

        // 지정한 개수(maxFpsCount)만큼 고주사율~저주사율 골고루 추리기
        List<int> selectedFpsList = new List<int>();
        int fpsTakeCount = Mathf.Min(maxFpsCount, sortedFpsList.Count);

        if (sortedFpsList.Count <= maxFpsCount)
        {
            selectedFpsList = sortedFpsList;
        }
        else
        {
            for (int i = 0; i < maxFpsCount; i++)
            {
                int index = (fpsTakeCount == 1) ? 0 : Mathf.RoundToInt((float)i / (fpsTakeCount - 1) * (sortedFpsList.Count - 1));
                selectedFpsList.Add(sortedFpsList[index]);
            }

            // 내림차순 정렬 유지
            selectedFpsList.Sort((a, b) => b.CompareTo(a));
        }

        fpsOptions = selectedFpsList.ToArray();
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

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    #region [ 해상도 조절 ]
    private void CreateResolutionUI()
    {
        // 기존에 생성된 자식 오브젝트가 있다면 정리
        foreach (Transform child in resolutionPanelContent)
        {
            Destroy(child.gameObject);
        }
        spawnedNodes.Clear();

        // 3. 현재 해상도를 검사해서 맞는 해상도 인덱스 찾기
        currentResoulutionSelectedIndex = GetCurrentResolutionIndex();

        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            GameObject nodeObj = Instantiate(resolutionNodePrefab, resolutionPanelContent);
            ResolutionNodes node = nodeObj.GetComponent<ResolutionNodes>();

            if (node != null)
            {
                // 2. index와 해상도 가로/세로 전달 및 텍스트 변경
                node.Initialize(i, resolutionOptions[i], this);

                // 3. 현재 해상도와 일치하는 프리팹은 화살표 표시
                if (i == currentResoulutionSelectedIndex)
                {
                    node.SetArrowsActive(true);
                }

                spawnedNodes.Add(node);
            }
        }
    }

    private int GetCurrentResolutionIndex()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            if (resolutionOptions[i].width == currentWidth && resolutionOptions[i].height == currentHeight)
            {
                return i;
            }
        }
        return 0; // 일치하는 해상도가 없을 경우 기본 첫 번째 값 반환
    }

    public void OnNodeHover(int index)
    {
        // 4. 특정 프리팹에 마우스가 올라갈 경우 해당 해상도만 화살표 표시, 나머지는 가리기
        for (int i = 0; i < spawnedNodes.Count; i++)
        {
            spawnedNodes[i].SetArrowsActive(i == index);
        }
    }

    public void OnNodeExit()
    {
        // 5. 모든 프리팹에서 마우스가 벗어날 경우 최종적으로 선택되어 있던 프리팹에만 화살표 표시
        for (int i = 0; i < spawnedNodes.Count; i++)
        {
            spawnedNodes[i].SetArrowsActive(i == currentResoulutionSelectedIndex);
        }
    }

    public void UpdateSelectedIndex(int index)
    {
        currentResoulutionSelectedIndex = index;
        // 6. 클릭 시 선택된 노드 고정 및 나머지 해제 갱신
        for (int i = 0; i < spawnedNodes.Count; i++)
        {
            spawnedNodes[i].SetArrowsActive(i == currentResoulutionSelectedIndex);
        }
    }

    #endregion

    #region [ FPS 조절 ]
    private void CreateFpsUI()
    {
        // 기존 생성된 FPS 자식 오브젝트 정리
        foreach (Transform child in fpsPanelContent)
        {
            Destroy(child.gameObject);
        }
        spawnedFpsNodes.Clear();

        // 현재 설정된 Application.targetFrameRate와 일치하는 인덱스 검사
        currentFpsSelectedIndex = GetCurrentFpsIndex();

        for (int i = 0; i < fpsOptions.Length; i++)
        {
            GameObject nodeObj = Instantiate(fpsNodePrefab, fpsPanelContent);
            FpsNodes node = nodeObj.GetComponent<FpsNodes>();

            if (node != null)
            {
                // 인덱스와 FPS 값 전달 및 "값 FPS" 형태로 텍스트 변경
                node.Initialize(i, fpsOptions[i], this);

                // 현재 적용된 FPS와 일치하는 경우 화살표 표시
                if (i == currentFpsSelectedIndex)
                {
                    node.SetArrowsActive(true);
                }

                spawnedFpsNodes.Add(node);
            }
        }
    }

    private int GetCurrentFpsIndex()
    {
        int currentFps = Application.targetFrameRate;

        // 만약 targetFrameRate가 설정되어 있지 않거나 기본값(-1 등)일 경우 처리 방어 코드
        for (int i = 0; i < fpsOptions.Length; i++)
        {
            if (fpsOptions[i] == currentFps)
            {
                return i;
            }
        }
        return 0; // 일치하는 값이 없으면 기본 첫 번째 인덱스 반환
    }

    public void OnFpsNodeHover(int index)
    {
        // 마우스가 올라간 FPS 노드만 화살표를 켜고 나머지는 끔 (해상도 노드에 영향 없음)
        for (int i = 0; i < spawnedFpsNodes.Count; i++)
        {
            spawnedFpsNodes[i].SetArrowsActive(i == index);
        }
    }

    public void OnFpsNodeExit()
    {
        // 마우스가 벗어나면 최종 선택된 FPS 노드에만 화살표 복원
        for (int i = 0; i < spawnedFpsNodes.Count; i++)
        {
            spawnedFpsNodes[i].SetArrowsActive(i == currentFpsSelectedIndex);
        }
    }

    public void UpdateFpsSelectedIndex(int index)
    {
        currentFpsSelectedIndex = index;
        // 클릭 시 선택된 FPS 노드 고정 및 나머지 해제
        for (int i = 0; i < spawnedFpsNodes.Count; i++)
        {
            spawnedFpsNodes[i].SetArrowsActive(i == currentFpsSelectedIndex);
        }
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

    #endregion
}

[System.Serializable]
public struct ResolutionOption
{
    public int width;
    public int height;
}
