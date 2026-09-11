using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // 에디터 모드에서도 동작하도록 설정
public class UIZoomController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelZoomIn;          // 줌인된 화면
    [SerializeField] private GameObject panelZoomOut;         // 줌아웃된 화면
    [SerializeField] private GameObject panelAnime;           // 연출 화면
    [SerializeField] private GameObject panelCookingUtensils; // 요리 도구들

    [Header("Zoom Panel")]
    [SerializeField] private List<ZoomSettings> zoomSettings; // 줌 설정 리스트

    [Header("Zoom Settings")]
    [SerializeField] private float zoomDuration = 0.5f;        // 이동 시간

    [Header("OrderManager")]
    [SerializeField] private OrderGameManager orderManager;    // OrderManager 참조

    private Coroutine[] currentCoroutines;

    private void Start()
    {
        // 에디터 상태가 아닌 실제 게임 플레이 시에만 초기화 실행
        if (!Application.isPlaying) return;

        panelZoomIn.SetActive(true);
        panelZoomOut.SetActive(false);
        panelCookingUtensils.SetActive(false);

        foreach (var setting in zoomSettings)
        {
            if (setting.targetPanel == null) continue;
            setting.targetPanel.localScale = setting.zoomInScale;
            setting.targetPanel.anchoredPosition = setting.zoomInPosition;
        }

        currentCoroutines = new Coroutine[zoomSettings.Count];
    }

    // ----------------------------------------------------
    // 에디터/런타임 공용 미리보기 적용 함수
    // ----------------------------------------------------

    public void ApplyZoomInState()
    {
        if (zoomSettings == null) return;

        foreach (var setting in zoomSettings)
        {
            if (setting.targetPanel == null) continue;

#if UNITY_EDITOR
            // 에디터 상에서 Undo(되돌리기) 지원 및 변경사항 저장 기록
            Undo.RecordObject(setting.targetPanel, "Apply Zoom In State");
#endif
            setting.targetPanel.localScale = setting.zoomInScale;
            setting.targetPanel.anchoredPosition = setting.zoomInPosition;
        }

        if (panelZoomIn != null) panelZoomIn.SetActive(true);
        if (panelZoomOut != null) panelZoomOut.SetActive(false);
        if (panelCookingUtensils != null) panelCookingUtensils.SetActive(false);
    }

    public void ApplyZoomOutState()
    {
        if (zoomSettings == null) return;

        foreach (var setting in zoomSettings)
        {
            if (setting.targetPanel == null) continue;

#if UNITY_EDITOR
            Undo.RecordObject(setting.targetPanel, "Apply Zoom Out State");
#endif
            setting.targetPanel.localScale = setting.zoomOutScale;
            setting.targetPanel.anchoredPosition = setting.zoomOutPosition;
        }

        if (panelZoomIn != null) panelZoomIn.SetActive(false);
        if (panelZoomOut != null) panelZoomOut.SetActive(true);
        if (panelCookingUtensils != null) panelCookingUtensils.SetActive(true);
    }

    // ----------------------------------------------------
    // 런타임 애니메이션 (기존 로직 유지)
    // ----------------------------------------------------

    public void ZoomIn()
    {
        if (!Application.isPlaying)
        {
            ApplyZoomInState();
            return;
        }

        if (panelZoomOut != null) panelZoomOut.SetActive(false);

        if (currentCoroutines == null || currentCoroutines.Length != zoomSettings.Count)
        {
            currentCoroutines = new Coroutine[zoomSettings.Count];
        }

        for (int i = 0; i < zoomSettings.Count; i++)
        {
            StartZoomAnimation(zoomSettings[i].targetPanel, zoomSettings[i].zoomInScale, zoomSettings[i].zoomInPosition, i, true);
        }
    }

    public void ZoomOut()
    {
        if (!Application.isPlaying)
        {
            ApplyZoomOutState();
            return;
        }

        if (!orderManager.IsSelectedFoodOk()) return;

        if (panelZoomIn != null) panelZoomIn.SetActive(false);
        if (panelCookingUtensils != null) panelCookingUtensils.SetActive(true);

        if (currentCoroutines == null || currentCoroutines.Length != zoomSettings.Count)
        {
            currentCoroutines = new Coroutine[zoomSettings.Count];
        }

        for (int i = 0; i < zoomSettings.Count; i++)
        {
            StartZoomAnimation(zoomSettings[i].targetPanel, zoomSettings[i].zoomOutScale, zoomSettings[i].zoomOutPosition, i, false);
        }
    }

    private void StartZoomAnimation(RectTransform targetPanel, Vector3 targetScale, Vector2 targetPos, int index, bool isZoomIn)
    {
        if (targetPanel == null) return;

        if (currentCoroutines[index] != null) StopCoroutine(currentCoroutines[index]);
        currentCoroutines[index] = StartCoroutine(AnimateZoom(targetPanel, targetScale, targetPos, isZoomIn));
    }

    private System.Collections.IEnumerator AnimateZoom(RectTransform targetPanel, Vector3 endScale, Vector2 endPos, bool isZoomIn)
    {
        Vector3 startScale = targetPanel.localScale;
        Vector2 startPos = targetPanel.anchoredPosition;
        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = time / zoomDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            targetPanel.localScale = Vector3.Lerp(startScale, endScale, t);
            targetPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        targetPanel.localScale = endScale;
        targetPanel.anchoredPosition = endPos;

        if (panelZoomIn != null) panelZoomIn.SetActive(isZoomIn);
        if (panelZoomOut != null) panelZoomOut.SetActive(!isZoomIn);

        if (isZoomIn && panelCookingUtensils != null)
        {
            panelCookingUtensils.SetActive(false);
        }
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

// ----------------------------------------------------
// 에디터 전용 커스텀 인스펙터 버튼 생성
// ----------------------------------------------------
#if UNITY_EDITOR
[CustomEditor(typeof(UIZoomController))]
public class UIZoomControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // 기존 인스펙터 요소 출력

        UIZoomController script = (UIZoomController)target;

        GUILayout.Space(15);
        EditorGUILayout.LabelField("Editor Preview Tools", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Preview Zoom In", GUILayout.Height(30)))
        {
            script.ApplyZoomInState();
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Preview Zoom Out", GUILayout.Height(30)))
        {
            script.ApplyZoomOutState();
            EditorUtility.SetDirty(script);
        }

        GUILayout.EndHorizontal();
    }
}
#endif