using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMover))]
public class NpcMoveScript : MonoBehaviour
{
    [Header("그리드 참조")]
    private Tilemap targetGrid; // 씬에 있는 Grid 오브젝트 할당

    private TileMover tileMover;
    private Vector3 previousTilePosition;

    // 아이소메트릭 그리드의 4방향 (인덱스 단위 방향)
    private readonly Vector2Int[] gridDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 상
        new Vector2Int(0, -1),  // 하
        new Vector2Int(-1, 0),  // 좌
        new Vector2Int(1, 0)    // 우
    };

    private void Awake()
    {
        tileMover = GetComponent<TileMover>();
    }

    private void Start()
    {
        targetGrid = BuildManager.Instance.PrivateTargetTilemap;
        previousTilePosition = transform.position;
        StartNextMove();
    }

    private void StartNextMove()
    {
        Vector3 currentPos = transform.position;

        // 1. 현재 월드 좌표를 Grid의 타일 좌표(Cell)로 변환
        Vector3Int currentCellPos = targetGrid.WorldToCell(currentPos);

        List<Vector3> candidateRoads = new List<Vector3>();

        // 2. Grid 기준으로 4방향 탐색
        foreach (Vector2Int dir in gridDirections)
        {
            // 다음 타일의 셀 좌표
            Vector3Int neighborCellPos = currentCellPos + new Vector3Int(dir.x, dir.y, 0);

            // 셀 좌표 -> 월드 좌표(타일의 중심점)로 변환
            Vector3 checkWorldPos = targetGrid.GetCellCenterWorld(neighborCellPos);

            // 해당 위치에 Road가 있는지 체크
            RoadNode road = BuildManager.Instance.GetRoadAt(checkWorldPos);
            if (road != null)
            {
                candidateRoads.Add(checkWorldPos);
            }
        }

        if (candidateRoads.Count == 0)
        {
            Invoke(nameof(StartNextMove), 1.0f);
            return;
        }

        // 3. 지나온 곳(previousTilePosition) 후순위 제외 처리
        List<Vector3> preferredRoads = new List<Vector3>();
        foreach (Vector3 roadPos in candidateRoads)
        {
            if (Vector3.Distance(roadPos, previousTilePosition) > 0.1f)
            {
                preferredRoads.Add(roadPos);
            }
        }

        Vector3 nextTargetPos;
        if (preferredRoads.Count > 0)
        {
            int randomIndex = Random.Range(0, preferredRoads.Count);
            nextTargetPos = preferredRoads[randomIndex];
        }
        else
        {
            // 막다른 길일 경우 지나온 길로 복귀
            nextTargetPos = candidateRoads[0];
        }

        // 4. 이동 수행
        previousTilePosition = currentPos;

        tileMover.MoveTo(nextTargetPos, OnReachedTileCenter);
    }

    private void OnReachedTileCenter()
    {
        StartNextMove();
    }
}
