using UnityEngine;

public class ClockUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private RectTransform hourHand; // 회전시킬 시침 RectTransform

    [Header("시간 설정")]
    private const float realSecondsPerDay = 86400f;  // 하루 24시간 = 86,400초
    private const float secondsPerHalfDay = 43200f;  // 12시간 = 43,200초

    [Tooltip("현재 하루 중 누적된 게임 시간 (0 ~ 86400초)")]
    public float currentDayTimeSeconds = 0f;

    // ClockManager로부터 가져온 '1초당 흐르는 게임 시간(초)' 비율
    private float timePerOnce = 1f;

    private bool isInitialized = false;

    private void Start()
    {
        InitializeTimeScale();
    }

    /// <summary>
    /// ClockManager를 찾아 TimePerOnce 비율 값을 가져옵니다.
    /// </summary>
    private void InitializeTimeScale()
    {
        if (((DayManager)TimeBase.Instance) != null)
        {
            timePerOnce = ((DayManager)TimeBase.Instance).TimePerOnce;
            isInitialized = true;
        }
        else
        {
            isInitialized = false;
        }

        if(TimeBase.Instance != null)
        {
            SetTimeDirectly(TimeBase.Instance.nowHour);
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 1초당 timePerOnce(게임 내 초)만큼 시간 증가
        currentDayTimeSeconds += timePerOnce * Time.deltaTime;

        // 24시간(86,400초)을 넘어가면 0초로 리셋
        if (currentDayTimeSeconds >= realSecondsPerDay)
        {
            currentDayTimeSeconds %= realSecondsPerDay;
        }

        // 시침 UI 회전 업데이트
        ApplyHourHandRotation();
    }

    /// <summary>
    /// [이벤트/외부 호출용] 특정 정수 시간(0~23시)으로 시침을 즉시 이동
    /// </summary>
    /// <param name="hour">설정할 시간 (0 ~ 23시)</param>
    public void SetTimeDirectly(int hour)
    {
        int clampedHour = Mathf.Clamp(hour, 0, 23);

        // 정수 시간(Hour)을 게임 내 초(Second) 단위로 변환 (1시간 = 3600초)
        currentDayTimeSeconds = clampedHour * 3600f;

        // 회전 적용
        ApplyHourHandRotation();
    }

    /// <summary>
    /// 12시간 시계 기준으로 currentDayTimeSeconds 기반 시침 Z축 회전 적용
    /// (0도가 6시를 가리키는 이미지 기준: +180도 보정)
    /// </summary>
    private void ApplyHourHandRotation()
    {
        // 12시간(43,200초) 동안의 진행률 (0.0 ~ 1.0)
        // % 연산자를 통해 12시(43,200초)가 지나면 진행률이 다시 0부터 시작됩니다 (오전/오후 분리)
        float progressHalfDay = (currentDayTimeSeconds % secondsPerHalfDay) / secondsPerHalfDay;

        // 12시간 시계 회전각 계산: 시계 방향(-), 6시 기준 이미지(+180도)
        float targetAngle = (-progressHalfDay * 360f) + 180f;

        // RectTransform 적용
        hourHand.localEulerAngles = new Vector3(0f, 0f, targetAngle);
    }
}