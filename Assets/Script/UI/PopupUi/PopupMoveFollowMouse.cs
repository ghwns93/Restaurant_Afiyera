using UnityEngine;

public class PopupMoveFollowMouse : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(10f, -10f); // 마우스와 팝업 사이의 미세 여백

    private RectTransform popupRectTransform; // 팝업창의 RectTransform
    private RectTransform parentRectTransform; // 부모의 rect
    private Canvas parentCanvas;

    private void Start()
    {
        popupRectTransform = GetComponent<RectTransform>();

        if (transform.parent != null)
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        // 팝업창이 켜져 있고, 마우스가 버튼 위에 있을 때만 마우스 위치를 추적
        if (popupRectTransform != null)
        {
            FollowMousePosition();
        }
    }

    // 마우스 위치로 팝업창을 이동시키는 메서드
    private void FollowMousePosition()
    {
        if (parentRectTransform == null || parentCanvas == null) return;

        Vector2 localPoint;
        Vector2 mousePos = Input.mousePosition;

        // 핵심: parentCanvas가 아니라, 팝업창의 '직속 부모(parentRectTransform)'를 기준으로 
        // 마우스의 스크린 좌표를 로컬 UI 좌표로 변환합니다.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            mousePos,
            parentCanvas.worldCamera,
            out localPoint))
        {
            // 부모 기준 좌표에 여백(Offset)을 더해 최종 위치를 결정합니다.
            popupRectTransform.anchoredPosition = localPoint + offset;
        }
    }
}
