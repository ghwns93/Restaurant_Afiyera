using UnityEngine;
using UnityEngine.Tilemaps;

public class MovedObjectSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Tilemap targetTilemap;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 씬에 있는 타일맵을 찾아서 연결 (또는 매니저를 통해 참조)
        targetTilemap = BuildManager.Instance.PrivateTargetTilemap;
    }

    private void LateUpdate()
    {
        if (targetTilemap == null) return;

        // 1. NPC의 '발바닥 위치'를 기준으로 정확한 타일 좌표를 얻습니다.
        // (NPC 스프라이트의 Pivot이 반드시 'Bottom Center'로 설정되어 있어야 합니다)
        Vector3 footPos = transform.position;
        Vector3Int cellPos = targetTilemap.WorldToCell(footPos);

        // 2. 울타리와 동일한 베이스 레이어를 계산합니다 (1000 단위 적용).
        int baseOrder = -(cellPos.x + cellPos.y) * 100;

        // 3. 울타리 뒷벽(0)과 앞벽(2) 사이에 NPC가 위치하도록 고정값 1을 더해줍니다.
        // 타일 경계를 넘어가는 순간 유니티 타일맵 좌표가 알아서 변경되므로 툭툭 끊기지 않습니다.
        spriteRenderer.sortingOrder = baseOrder + 1;
    }
}
