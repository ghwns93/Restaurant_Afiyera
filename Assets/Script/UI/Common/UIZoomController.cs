using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIZoomController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelZoomIn;  // 줌인된 화면
    [SerializeField] private GameObject panelZoomOut; // 줌아웃된 화면
    [SerializeField] private GameObject panelAnime;   // 연출 화면

    [Header("Zoom Panel")]
    [SerializeField] private List<ZoomSettings> zoomSettings; // 줌 설정 리스트

    [Header("Zoom Settings")]
    [SerializeField] private float zoomDuration = 0.5f; // 이동 시간

    private Coroutine[] currentCoroutines;

    private void Start()
    {
        // 초기 상태 설정
        panelZoomIn.SetActive(true);
        panelZoomOut.SetActive(false);
        panelAnime.SetActive(true);
        // 각 패널의 초기 위치와 스케일 설정
        foreach (var setting in zoomSettings)
        {
            setting.targetPanel.localScale = setting.zoomInScale;
            setting.targetPanel.anchoredPosition = setting.zoomInPosition;
        }
        panelAnime.SetActive(false);

        currentCoroutines = new Coroutine[zoomSettings.Count];
    }

    // 빨간 버튼 OnClick()에 연결
    public void ZoomIn()
    {
        panelZoomOut.SetActive(false);

        int i = 0;

        foreach (var setting in zoomSettings)
        {
            StartZoomAnimation(setting.targetPanel, setting.zoomInScale, setting.zoomInPosition, i);
            i++;
        }
        panelZoomIn.SetActive(true);
    }

    // 초록 버튼 OnClick()에 연결
    public void ZoomOut()
    {
        panelZoomIn.SetActive(false);

        int i = 0;

        foreach (var setting in zoomSettings)
        {
            StartZoomAnimation(setting.targetPanel, setting.zoomOutScale, setting.zoomOutPosition, i);
            i++;
        }
        panelZoomOut.SetActive(true);
    }

    private void StartZoomAnimation(RectTransform targetPanel, Vector3 targetScale, Vector2 targetPos, int index)
    {
        if (currentCoroutines[index] != null) StopCoroutine(currentCoroutines[index]);
        currentCoroutines[index] = StartCoroutine(AnimateZoom(targetPanel, targetScale, targetPos));
    }

    private IEnumerator AnimateZoom(RectTransform targetPanel, Vector3 endScale, Vector2 endPos)
    {
        panelAnime.SetActive(true);

        Vector3 startScale = targetPanel.localScale;
        Vector2 startPos = targetPanel.anchoredPosition;
        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = time / zoomDuration;
            // SmoothStep으로 부드럽게 감속 연출
            t = Mathf.SmoothStep(0f, 1f, t);

            targetPanel.localScale = Vector3.Lerp(startScale, endScale, t);
            targetPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        targetPanel.localScale = endScale;
        targetPanel.anchoredPosition = endPos;

        panelAnime.SetActive(false);
    }
}

[System.Serializable]
public struct ZoomSettings
{
    public RectTransform targetPanel;
    public Vector3 zoomInScale;
    public Vector2 zoomInPosition;
    public Vector3 zoomOutScale;
    public Vector2 zoomOutPosition;
}