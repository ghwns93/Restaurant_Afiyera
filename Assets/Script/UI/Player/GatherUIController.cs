using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GatherUIController : MonoBehaviour
{
    private static GatherUIController instance;
    public static GatherUIController Instance => instance;

    [SerializeField] private GameObject gatherUIPanel;          // 채집 안내 UI 전체 패널 (PlayerInteraction)
    [SerializeField] private Image fillBackgroundImage;         // 360도 채워질 BackGround 이미지 (Image Type: Filled)
    [SerializeField] private TextMeshProUGUI keyText;           // 단축키 표시 텍스트 (Text (TMP))
    [SerializeField] private TextMeshProUGUI actionText;        // 행동 표시 텍스트 (InteractionDescription 등)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HideGatherUI();
    }

    // 채집 UI 표시 및 초기화
    public void ShowGatherUI(KeyCode currentKey, string actionName = "채집")
    {
        if (gatherUIPanel != null)
        {
            gatherUIPanel.SetActive(true);
        }

        if (keyText != null)
        {
            keyText.text = GetKeyName(currentKey);
        }

        if (actionText != null)
        {
            actionText.text = actionName;
        }

        // 처음 켜졌을 때는 프로그레스바를 0으로 초기화
        SetProgress(0f);
    }

    // 채집 UI 숨기기
    public void HideGatherUI()
    {
        if (gatherUIPanel != null)
        {
            gatherUIPanel.SetActive(false);
        }

        SetProgress(0f);
    }

    // 프로그레스바 채우기 (0.0 ~ 1.0 값 전달)
    public void SetProgress(float progress)
    {
        if (fillBackgroundImage != null)
        {
            // Mathf.Clamp00 대신 Mathf.Clamp(값, 최소값, 최대값) 사용
            fillBackgroundImage.fillAmount = Mathf.Clamp(progress, 0f, 1f);
        }
    }

    private string GetKeyName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";
            case KeyCode.Alpha0: return "0";
            default: return key.ToString();
        }
    }
}