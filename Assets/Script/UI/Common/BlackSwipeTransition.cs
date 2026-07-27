using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackSwipeTransition : MonoBehaviour
{
    [Header("Transform Targets (위치 기준)")]
    [SerializeField] private RectTransform targetA; // A 배경의 RectTransform
    [SerializeField] private RectTransform targetB; // B 배경의 RectTransform

    [Header("UI Overlay (Fader)")]
    [SerializeField] private RectTransform faderRT; // TransitionFader

    [Header("Settings")]
    [SerializeField] private float swipeDuration = 0.4f;

    private float viewportHeight;
    private bool isAtA = true;
    private bool isTransitioning = false;

    private void Start()
    {
        // 캔버스(화면) 자체의 높이 구하기
        Canvas parentCanvas = faderRT.GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            viewportHeight = parentCanvas.GetComponent<RectTransform>().rect.height;
        }

        ResetFader();
    }

    public void OnClickSwitchBackground()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;
        faderRT.SetAsLastSibling(); // 검은 막을 최상단으로

        // --------------------------------------------------
        // Phase 1: 검은색이 위에서 아래로 가득 참
        // --------------------------------------------------
        faderRT.anchoredPosition = Vector2.zero;
        float elapsedTime = 0f;

        while (elapsedTime < swipeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / swipeDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            SetFaderHeight(Mathf.Lerp(0f, viewportHeight, smoothT));
            yield return null;
        }
        SetFaderHeight(viewportHeight);

        // --------------------------------------------------
        // Phase 2: 검은 화면일 때 위치 이동 (A -> B 또는 B -> A)
        // --------------------------------------------------
        Vector2 targetPosition = targetB.anchoredPosition;

        targetB.anchoredPosition = targetA.anchoredPosition;
        targetA.anchoredPosition = targetPosition;

        isAtA = !isAtA;
        yield return new WaitForSeconds(0.05f);

        // --------------------------------------------------
        // Phase 3: 위에서 아래로 풀림 (아래로 내려가며 사라짐)
        // --------------------------------------------------
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(0f, -viewportHeight);

        elapsedTime = 0f;
        while (elapsedTime < swipeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / swipeDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            SetFaderHeight(Mathf.Lerp(viewportHeight, 0f, smoothT));
            faderRT.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);

            yield return null;
        }

        // --------------------------------------------------
        // 완료 후 Reset
        // --------------------------------------------------
        ResetFader();
        isTransitioning = false;
    }

    private void SetFaderHeight(float height)
    {
        faderRT.sizeDelta = new Vector2(faderRT.sizeDelta.x, height);
    }

    private void ResetFader()
    {
        SetFaderHeight(0f);
        faderRT.anchoredPosition = Vector2.zero;
    }
}