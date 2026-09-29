using UnityEngine;

public class PlayerOverlayManager : MonoBehaviour
{
    [Tooltip("장애물로 처리할 오브젝트들의 태그 (예: Obstacle)")]
    [SerializeField] private string obstacleTag = "Obstacle";

    [Tooltip("트리거로 사용할 콜라이더의 크기 (박스 콜라이더 기준)")]
    [SerializeField] private Vector2 defaultBoxSize = new Vector2(1f, 1f);

    [Range(0f, 1f)]
    [Tooltip("장애물 오버레이의 투명도 (0: 완전히 투명, 1: 불투명)")]
    [SerializeField] private float overlayAlpha = 0.5f;

    private void Awake()
    {
        SetupObstacles();
    }

    private void SetupObstacles()
    {
        // 씬 내에 해당 태그가 걸린 모든 오브젝트를 찾습니다.
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);

        foreach (var obs in obstacles)
        {
            // 1. SpriteRenderer가 없으면 패스
            SpriteRenderer sr = obs.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            // 2. BoxCollider2D 추가
            BoxCollider2D boxCol = obs.AddComponent<BoxCollider2D>();
            boxCol.size = defaultBoxSize;
            boxCol.isTrigger = true;

            // 3. ObstacleOverlayHandler 컴포넌트가 없으면 추가
            ObstacleOverlayHandler handler = obs.GetComponent<ObstacleOverlayHandler>();
            if (handler == null)
            {
                obs.AddComponent<ObstacleOverlayHandler>();

                obs.GetComponent<ObstacleOverlayHandler>().overlayAlpha = overlayAlpha;
            }
        }
    }
}