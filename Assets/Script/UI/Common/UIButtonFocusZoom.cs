using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFocusZoom : MonoBehaviour
{
    [Header("Camera & Canvas Settings")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Canvas targetCanvas; // 사용 중인 Canvas를 할당해주세요.

    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float paddingMultiplier = 1.05f; // 여백 비율 (1.0 = 딱 맞춤, 1.1 = 10% 여백)

    private Vector3 originalCameraPosition;
    private float originalOrthographicSize;
    private Coroutine zoomCoroutine;

    private bool isZoomedIn = false;
    private GameObject zoomedBtn;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCanvas == null)
            targetCanvas = GetComponentInParent<Canvas>();

        // 초기 카메라 상태 저장
        originalCameraPosition = targetCamera.transform.position;
        originalOrthographicSize = targetCamera.orthographicSize;
    }

    private void Update()
    {
        // 예시: 마우스 오른쪽 버튼 클릭 시 원래 상태로 복귀
        if (Input.GetMouseButtonDown(1) && isZoomedIn)
        {
            ResetZoom();
        }
    }

    /// <summary>
    /// UI Button OnClick()에 연결하는 메서드 (Button 자신을 인수로 전달)
    /// </summary>
    public void ZoomToButton(Button targetButton)
    {
        RectTransform buttonRect = targetButton.GetComponent<RectTransform>();
        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();

        // 1. 카메라 이동 목표 위치 (버튼의 월드 Position 사용, Z축은 유지)
        Vector3 targetPosition = new Vector3(
            buttonRect.position.x,
            buttonRect.position.y,
            targetCamera.transform.position.z
        );

        // 2. Canvas 대비 버튼의 비율(Ratio) 계산 (Canvas Scaler/Scale 영향 배제)
        float buttonWidthOnCanvas = buttonRect.rect.width * buttonRect.lossyScale.x / canvasRect.lossyScale.x;
        float buttonHeightOnCanvas = buttonRect.rect.height * buttonRect.lossyScale.y / canvasRect.lossyScale.y;

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // 버튼이 전체 Canvas에서 차지하는 비율 (0 ~ 1)
        float widthRatio = buttonWidthOnCanvas / canvasWidth;
        float heightRatio = buttonHeightOnCanvas / canvasHeight;

        // 3. 화면 꽉 차게 만들 Orthographic Size 계산
        // Screen Space - Camera에서 전체 화면을 담는 기본 Size는 캔버스의 높이 기준 비율에 의해 결정됩니다.
        float aspect = targetCamera.aspect;

        // 세로 기준 / 가로 기준 비율 중 더 크게 차지하는 쪽으로 카메라 Size 조절
        float targetSizeForHeight = originalOrthographicSize * heightRatio;
        float targetSizeForWidth = (originalOrthographicSize * widthRatio) * ((canvasWidth / canvasHeight) / aspect);

        float targetSize = Mathf.Max(targetSizeForHeight, targetSizeForWidth) * paddingMultiplier;

        // 4. 애니메이션 실행
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(Co_AnimateCamera(targetPosition, targetSize));

        isZoomedIn = true;

        zoomedBtn = targetButton.gameObject;
        zoomedBtn.SetActive(false); // 버튼 비활성화
    }

    /// <summary>
    /// 원래 상태로 복귀
    /// </summary>
    public void ResetZoom()
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(Co_AnimateCamera(originalCameraPosition, originalOrthographicSize));

        isZoomedIn = false;
        zoomedBtn.SetActive(true); // 버튼 비활성화
        zoomedBtn = null;
    }

    private IEnumerator Co_AnimateCamera(Vector3 targetPos, float targetSize)
    {
        Vector3 startPos = targetCamera.transform.position;
        float startSize = targetCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            targetCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            targetCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        targetCamera.transform.position = targetPos;
        targetCamera.orthographicSize = targetSize;
    }
}