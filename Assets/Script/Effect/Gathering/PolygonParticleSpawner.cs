using UnityEngine;
using System.Collections.Generic;

public class PolygonParticleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private GameObject starParticlePrefab; // 별 모양 이미지가 붙은 프리팹

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 0.5f;   // 생성 주기
    [SerializeField] private float minSpeed = 1f;          // 올라가는 최소 속도
    [SerializeField] private float maxSpeed = 2.5f;        // 올라가는 최대 속도
    [SerializeField] private float maxLifeTime = 2.0f;     // 최대 생존 시간 (위로 올라가다 사라지는 기준)

    private float spawnTimer;
    private Bounds colliderBounds;

    private void Start()
    {
        if (polygonCollider == null)
        {
            polygonCollider = GetComponent<PolygonCollider2D>();
        }

        if (polygonCollider != null)
        {
            colliderBounds = polygonCollider.bounds;
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnStar();
        }
    }

    private void SpawnStar()
    {
        if (polygonCollider == null) return;

        // 1. 폴리곤 콜라이더 내부의 유효한 랜덤 포인트 찾기
        Vector2 randomPoint = GetRandomPointInPolygon(polygonCollider);

        // 2. 별 이펙트 오브젝트 생성
        if (starParticlePrefab != null)
        {
            GameObject star = Instantiate(starParticlePrefab, randomPoint, Quaternion.identity);

            // 위로 떠오르며 페이드 아웃되는 컴포넌트 부착 또는 실행
            StarEffectItem starItem = star.GetComponent<StarEffectItem>();
            if (starItem == null)
            {
                starItem = star.AddComponent<StarEffectItem>();
            }

            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            starItem.Initialize(randomSpeed, maxLifeTime);
        }
    }

    // 폴리곤 콜라이더 내부 랜덤 점 추출 함수 (Bounding Box 내에서 내부 판정)
    private Vector2 GetRandomPointInPolygon(PolygonCollider2D poly)
    {
        Bounds bounds = poly.bounds;
        Vector2 randomPoint = Vector2.zero;
        bool isInside = false;
        int maxTrials = 30; // 무한 루프 방지

        for (int i = 0; i < maxTrials; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            randomPoint = new Vector2(randomX, randomY);

            // 해당 점이 Polygon Collider 2D 내부에 있는지 확인
            if (poly.OverlapPoint(randomPoint))
            {
                isInside = true;
                break;
            }
        }

        // 만약 못 찾았다면 콜라이더 중심점 반환
        if (!isInside)
        {
            randomPoint = poly.bounds.center;
        }

        return randomPoint;
    }
}