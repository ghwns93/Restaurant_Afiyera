using System.Collections;
using UnityEngine;

public class UIZoomController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelZoomIn;  // 줌인된 화면
    [SerializeField] private GameObject panelZoomOut; // 줌아웃된 화면
    [SerializeField] private GameObject panelAnime;   // 연출 화면

    [Header("Target Panel")]
    [SerializeField] private RectTransform viewPanel; // 배경 및 요소들이 들어있는 부모 Panel

    [Header("Zoom Settings")]
    [SerializeField] private float zoomDuration = 0.5f; // 이동 시간

    // 첫 번째 화면 상태 (기본값)
    private Vector3 defaultScale = Vector3.one;
    private Vector2 defaultPosition = Vector2.zero;

    // 두 번째 화면 상태 (줌인 목표값 - 에디터에서 맞춘 값을 인스펙터로 조정 가능)
    [Header("Zoom In Target Values")]
    [SerializeField] private Vector3 targetScale = new Vector3(2.5f, 2.5f, 1f);
    [SerializeField] private Vector2 targetPosition = new Vector2(0f, -300f);

    private Coroutine currentCoroutine;

    private void Start()
    {
        viewPanel.localScale = targetScale;
        viewPanel.anchoredPosition = targetPosition;
    }

    // 빨간 버튼 OnClick()에 연결
    public void ZoomIn()
    {
        panelZoomIn.SetActive(false);
        StartZoomAnimation(targetScale, targetPosition);
        panelZoomOut.SetActive(true);
    }

    // 초록 버튼 OnClick()에 연결
    public void ZoomOut()
    {
        panelZoomOut.SetActive(false);
        StartZoomAnimation(defaultScale, defaultPosition);
        panelZoomIn.SetActive(true);
    }

    private void StartZoomAnimation(Vector3 targetScale, Vector2 targetPos)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateZoom(targetScale, targetPos));
    }

    private IEnumerator AnimateZoom(Vector3 endScale, Vector2 endPos)
    {
        panelAnime.SetActive(true);

        Vector3 startScale = viewPanel.localScale;
        Vector2 startPos = viewPanel.anchoredPosition;
        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = time / zoomDuration;
            // SmoothStep으로 부드럽게 감속 연출
            t = Mathf.SmoothStep(0f, 1f, t);

            viewPanel.localScale = Vector3.Lerp(startScale, endScale, t);
            viewPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        viewPanel.localScale = endScale;
        viewPanel.anchoredPosition = endPos;

        panelAnime.SetActive(false);
    }
}