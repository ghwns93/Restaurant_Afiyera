using System.Collections;
using UnityEngine;

public class UIFrameBoundZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera uiCamera;
    [SerializeField] private float zoomDuration = 0.5f;          // 줌 전환 시간
    [SerializeField] private float padding = 50.0f;              // 여백 (픽셀 단위)
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float initialOrthographicSize;
    private Vector3 initialCameraPosition;
    private Coroutine currentZoomCoroutine;
    private bool isZoomedIn = false;

    private void Awake()
    {
        if (uiCamera == null)
        {
            uiCamera = GetComponent<Camera>();
        }

        // 초기 카메라 상태 저장 (줌아웃 복구용)
        initialOrthographicSize = uiCamera.orthographicSize;
        initialCameraPosition = uiCamera.transform.position;
    }

    /// <summary>
    /// 지정한 두 UI Bound 영역에 맞춰 카메라 줌인
    /// </summary>
    public void ZoomToUIBounds(RectTransform boundA, RectTransform boundB)
    {
        if (boundA == null || boundB == null) return;

        // 1. 두 UI의 World Corners 좌표 구하기
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        boundA.GetWorldCorners(cornersA);
        boundB.GetWorldCorners(cornersB);

        // 2. 두 UI를 포함하는 전체 영역의 최소/최대 좌표 계산
        float minX = Mathf.Min(GetMinX(cornersA), GetMinX(cornersB));
        float maxX = Mathf.Max(GetMaxX(cornersA), GetMaxX(cornersB));
        float minY = Mathf.Min(GetMinY(cornersA), GetMinY(cornersB));
        float maxY = Mathf.Max(GetMaxY(cornersA), GetMaxY(cornersB));

        // 3. 중심 위치 계산
        Vector3 targetCenter = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, uiCamera.transform.position.z);

        // 4. 가로/세로 폭 및 필요한 Orthographic Size 계산
        float width = (maxX - minX) + (padding * 2f);
        float height = (maxY - minY) + (padding * 2f);

        float screenAspect = (float)Screen.width / Screen.height;

        // 가로 비율과 세로 비율 중 더 넓은 쪽을 기준으로 Size 결정
        float sizeBasedOnWidth = (width / 2f) / screenAspect;
        float sizeBasedOnHeight = height / 2f;
        float targetOrthoSize = Mathf.Max(sizeBasedOnWidth, sizeBasedOnHeight);

        // 5. 줌 연출 실행
        if (currentZoomCoroutine != null) StopCoroutine(currentZoomCoroutine);
        currentZoomCoroutine = StartCoroutine(AnimateCameraZoom(targetCenter, targetOrthoSize));

        isZoomedIn = true;
    }

    /// <summary>
    /// 원래 화면 카메라 상태로 줌아웃 (복구)
    /// </summary>
    public void ResetZoom()
    {
        if (!isZoomedIn) return;

        if (currentZoomCoroutine != null) StopCoroutine(currentZoomCoroutine);
        currentZoomCoroutine = StartCoroutine(AnimateCameraZoom(initialCameraPosition, initialOrthographicSize));

        isZoomedIn = false;
    }

    private IEnumerator AnimateCameraZoom(Vector3 targetPos, float targetSize)
    {
        Vector3 startPos = uiCamera.transform.position;
        float startSize = uiCamera.orthographicSize;
        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = zoomCurve.Evaluate(time / zoomDuration);

            uiCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            uiCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        uiCamera.transform.position = targetPos;
        uiCamera.orthographicSize = targetSize;
    }

    // Helper functions for bounds
    private float GetMinX(Vector3[] corners) => Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
    private float GetMaxX(Vector3[] corners) => Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
    private float GetMinY(Vector3[] corners) => Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
    private float GetMaxY(Vector3[] corners) => Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
}