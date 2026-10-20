using UnityEngine;
using Spine.Unity; // Spine 관련 컴포넌트를 사용하기 위해 필요

public class SortingOrderController : MonoBehaviour
{
    [SerializeField] private MeshRenderer frontSkeletonRenderer;
    [SerializeField] private MeshRenderer backSkeletonRenderer;

    private SpriteRenderer spriteRenderer;

    private int originalSortingOrder;
    private bool isSpine = false; // 현재 스파인 캐릭터인지 판별하는 플래그

    private void Start()
    {
        if (frontSkeletonRenderer != null)
        {
            isSpine = true;
            originalSortingOrder = frontSkeletonRenderer.sortingOrder;
        }
        else
        {
            isSpine = false;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void LateUpdate()
    {
        // 렌더러가 아예 없다면 실행 중지
        if (isSpine && frontSkeletonRenderer == null) return;
        if (isSpine && backSkeletonRenderer == null) return;
        if (!isSpine && spriteRenderer == null) return;

        // 1. 발바닥 위치 기준 Y좌표 계산
        Vector3 footPos = transform.position;

        // 2. 베이스 레이어 계산 (1000 단위 등 게임 환경에 맞게 조절)
        int baseOrder = Mathf.RoundToInt(-(footPos.y) * 100 - 150);

        // 3. 컴포넌트 종류에 따라 각각 다른 방식으로 sortingOrder 적용
        if (isSpine)
        {
            // --- Spine (SkeletonRenderer)인 경우 처리 방식 ---
            frontSkeletonRenderer.sortingOrder = baseOrder + originalSortingOrder;
            backSkeletonRenderer.sortingOrder = baseOrder + originalSortingOrder;
        }
        else
        {
            spriteRenderer.sortingOrder = baseOrder + originalSortingOrder;
        }
    }
}