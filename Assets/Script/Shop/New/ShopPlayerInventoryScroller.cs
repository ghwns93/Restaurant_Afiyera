using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopPlayerInventoryScroller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform contentRect; // Horizontal Layout Group이 들어있는 Content 오브젝트
    [SerializeField] private HorizontalLayoutGroup layoutGroup; // 레이아웃 그룹 컴포넌트
    [SerializeField] private RectTransform viewportRect; // Mask(Rect Mask 2D)가 있는 부모 뷰포트 (영역 계산용)

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.2f; // 한 칸 이동하는 데 걸리는 시간 (초)

    private bool isSliding = false;
    private float targetXPosition = 0f;

    private void Start()
    {
        if (contentRect == null)
        {
            contentRect = GetComponent<RectTransform>();
        }
        if (layoutGroup == null)
        {
            layoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        // Viewport가 연결되지 않았다면 Content의 부모를 Viewport로 자동 지정
        if (viewportRect == null && contentRect.parent != null)
        {
            viewportRect = contentRect.parent.GetComponent<RectTransform>();
        }

        targetXPosition = contentRect.anchoredPosition.x;
    }

    // 오른쪽 버튼 클릭 시 (콘텐츠가 왼쪽으로 이동)
    public void OnClickNext()
    {
        if (isSliding) return;
        if (contentRect.childCount == 0) return;

        float moveAmount = GetSlotStepSize();

        // 최대 이동 가능한 한계치 계산 (오른쪽 끝에 도달했을 때 더 안 가도록)
        float maxScrollX = GetMaxScrollLimit();

        // 다음 위치가 최대 이동 범위를 넘지 않도록 제한
        if (targetXPosition - moveAmount < maxScrollX)
        {
            targetXPosition = maxScrollX; // 딱 끝부분에 맞춤
        }
        else
        {
            targetXPosition -= moveAmount;
        }

        StartCoroutine(SlideToPosition(targetXPosition));
    }

    // 왼쪽 버튼 클릭 시 (콘텐츠가 오른쪽으로 이동)
    public void OnClickPrev()
    {
        if (isSliding) return;
        if (contentRect.childCount == 0) return;

        float moveAmount = GetSlotStepSize();
        targetXPosition += moveAmount;

        // 시작점(맨 왼쪽, 0)을 넘어서 공백이 생기지 않도록 제한
        targetXPosition = Mathf.Min(targetXPosition, 0f);

        StartCoroutine(SlideToPosition(targetXPosition));
    }

    // 아이템 1칸의 크기 + 레이아웃 간격(Spacing) 계산
    private float GetSlotStepSize()
    {
        RectTransform slotRect = contentRect.GetChild(0) as RectTransform;
        float slotWidth = slotRect.rect.width;
        float spacing = layoutGroup != null ? layoutGroup.spacing : 0f;

        return slotWidth + spacing;
    }

    // 오른쪽 끝으로 더 이상 갈 수 없는 최대 X 좌표 계산
    private float GetMaxScrollLimit()
    {
        if (viewportRect == null) return 0f;

        float totalContentWidth = 0f;
        int activeChildCount = 0;

        // 활성화된 자식 아이템들의 총 너비 계산
        for (int i = 0; i < contentRect.childCount; i++)
        {
            RectTransform child = contentRect.GetChild(i) as RectTransform;
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
                totalContentWidth += child.rect.width;
            }
        }

        // 아이템 간의 Spacing 합산
        if (activeChildCount > 1 && layoutGroup != null)
        {
            totalContentWidth += layoutGroup.spacing * (activeChildCount - 1);
        }

        // Padding 고려
        if (layoutGroup != null)
        {
            totalContentWidth += layoutGroup.padding.left + layoutGroup.padding.right;
        }

        float viewportWidth = viewportRect.rect.width;

        // 전체 콘텐츠 너비가 뷰포트보다 작거나 같으면 이동할 필요 없음 (0 반환)
        if (totalContentWidth <= viewportWidth) return 0f;

        // 음수 값으로 최대 이동 가능 거리 반환
        return -(totalContentWidth - viewportWidth);
    }

    private IEnumerator SlideToPosition(float targetX)
    {
        isSliding = true;
        float startX = contentRect.anchoredPosition.x;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / slideDuration);
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease Out

            float currentX = Mathf.Lerp(startX, targetX, t);
            contentRect.anchoredPosition = new Vector2(currentX, contentRect.anchoredPosition.y);

            yield return null;
        }

        contentRect.anchoredPosition = new Vector2(targetX, contentRect.anchoredPosition.y);
        isSliding = false;
    }
}
