using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // 에디터에서도 확인하고 싶을 때
public class ResponsiveGrid : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    // 오브젝트의 크기(해상도)가 변할 때만 유니티가 자동으로 호출
    void OnRectTransformDimensionsChange()
    {
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        if (grid == null || rectTransform == null) return;

        float width = rectTransform.rect.width;
        // 여백과 간격을 제외한 순수 아이템 영역 계산
        float totalSpacing = grid.spacing.x * (grid.constraintCount - 1);
        float totalPadding = grid.padding.left + grid.padding.right;

        float newSize = (width - totalPadding - totalSpacing) / grid.constraintCount;

        if (newSize > 0)
            grid.cellSize = new Vector2(newSize, newSize);
    }
}