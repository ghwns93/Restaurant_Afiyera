using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMover))]
public class NPCDestinationMover : MonoBehaviour
{
    private Tilemap targetGrid;

    private TileMover tileMover;
    private List<Vector3Int> currentPath;
    private int currentPathIndex = 0;

    [SerializeField] private List<SpecificTimeNpcMoveInfo> specificTimeMoveInfos = new List<SpecificTimeNpcMoveInfo>();

    private void Awake()
    {
        tileMover = GetComponent<TileMover>();

        TimeEvents.OnNpcSpecificTimeReached += SpecificTimeCheck;
    }

    private void Start()
    {
        targetGrid = BuildManager.Instance.PrivateTargetTilemap;
    }

    private void OnDisable()
    {
        TimeEvents.OnNpcSpecificTimeReached -= SpecificTimeCheck;
    }

    //테스트용 마우스 클릭
    //private void Update()
    //{
    //    if(Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        mouseWorldPos.z = 0; // Z축을 0으로 설정하여 2D 평면에서 이동하도록 함
    //        SetDestination(mouseWorldPos);
    //    }
    //}

    private void SpecificTimeCheck(int hour)
    {
        foreach(var moveInfo in specificTimeMoveInfos)
        {
            if(moveInfo.hour == hour)
            {
                SetDestination(moveInfo.destination);
                break;
            }
        }
    }

    /// <summary>
    /// 외부에서 이 메서드를 호출해 원하는 목적지로 NPC를 이동시킵니다.
    /// </summary>
    public void SetDestination(Vector3 destinationWorldPos)
    {
        Vector3Int startCell = targetGrid.WorldToCell(transform.position);
        Vector3Int targetCell = targetGrid.WorldToCell(destinationWorldPos);

        // A* 알고리즘으로 경로 계산
        currentPath = Pathfinder.FindPath(targetGrid, startCell, targetCell);

        if (currentPath != null && currentPath.Count > 0)
        {
            currentPathIndex = 0;
            MoveToNextTile();
        }
        else
        {
            Debug.LogWarning("목적지까지 이어지는 Road 경로를 찾을 수 없습니다!");
        }
    }

    private void MoveToNextTile()
    {
        // 모든 경로를 다 이동했으면 정지
        if (currentPathIndex >= currentPath.Count)
        {
            OnArrival();
            return;
        }

        // 경로의 다음 타일 중심 월드 좌표 구하기
        Vector3Int nextCell = currentPath[currentPathIndex];
        Vector3 nextWorldPos = targetGrid.GetCellCenterWorld(nextCell);

        currentPathIndex++;

        // TileMover를 사용해 이동
        tileMover.MoveTo(nextWorldPos, MoveToNextTile);
    }

    private void OnArrival()
    {
        Debug.Log("목적지에 성공적으로 도착했습니다!");
        // 도착 후 수행할 동작 (예: 상점 이용, 상호작용 등)
    }
}

[System.Serializable]
public struct SpecificTimeNpcMoveInfo
{
    public int hour; // 이동할 시간 (0~23)
    public Vector3 destination; // 이동할 목적지 월드 좌표
    public SpecificTimeNpcMoveInfo(int hour, Vector3 destination)
    {
        this.hour = hour;
        this.destination = destination;
    }
}