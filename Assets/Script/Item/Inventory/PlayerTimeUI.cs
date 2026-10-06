using TMPro;
using UnityEngine;

public class PlayerTimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hourText;   // 시간을 표시할 TextMeshProUGUI 컴포넌트
    [SerializeField] private TextMeshProUGUI minuteText; // 분을 표시할 TextMeshProUGUI 컴포넌트

    private void OnEnable()
    {
        TimeEvents.OnTimeChanged += UpdateTimeUI;
    }

    private void OnDisable()
    {
        TimeEvents.OnTimeChanged -= UpdateTimeUI;
    }

    private void Start()
    {
        UpdateTimeUI();
    }

    private void UpdateTimeUI()
    {
        if(TimeBase.Instance != null)
        {
            hourText.text = TimeBase.Instance.nowHour.ToString("D2");
            minuteText.text = TimeBase.Instance.nowMinute.ToString("D2");
        }
    }
}
