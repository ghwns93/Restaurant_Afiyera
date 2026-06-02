using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [SerializeField] private Tilemap privateTargetTilemap;
    [SerializeField] private Vector3 baseAnchor;

    // [추가] 상하좌우 4방향을 체크하기 위한 방향 벡터 배열
    private readonly Vector3Int[] privateDirections = {
        new Vector3Int(1, 0, 0),  // 우
        new Vector3Int(-1, 0, 0), // 좌
        new Vector3Int(0, 1, 0),  // 상
        new Vector3Int(0, -1, 0)  // 하
    };

    // 만약 대각선 포함 8방향이 필요하다면 아래처럼 구성합니다.
    private readonly Vector3Int[] privateDirections8 = {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)
    };

    // 모든 건물/울타리를 BasicNode 타입으로 통합 관리
    private Dictionary<Vector3Int, BasicNode> privateAllNodes = new Dictionary<Vector3Int, BasicNode>();
    private List<BasicNode> privateCachedNodeList = new List<BasicNode>();
    private bool privateIsDirty = true; // 리스트 갱신이 필요한지 체크하는 플래그

    public Tilemap PrivateTargetTilemap { get => privateTargetTilemap; set => privateTargetTilemap = value; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // 특정 좌표에 건물이 있는지 확인 (가장 중요한 체크 함수)
    public bool HasNodeAt(Vector3Int pos)
    {
        return privateAllNodes.ContainsKey(pos);
    }

    // 특정 좌표에 특정타입의 건물이 있는지 확인
    public bool HasNodeAt<T>(Vector3Int pos) where T : BasicNode
    {
        if (privateAllNodes.TryGetValue(pos, out BasicNode node))
        {
            return node is T;
        }
        return false;
    }

    // 키값을 통해 특정 노드를 가져오는 함수
    public T GetNodeAt<T>(Vector3Int pos) where T : BasicNode
    {
        if (privateAllNodes.TryGetValue(pos, out BasicNode node))
        {
            return node as T;
        }
        return null;
    }

    public List<BasicNode> GetAllNodes()
    {
        if (privateIsDirty)
        {
            privateCachedNodeList = new List<BasicNode>(privateAllNodes.Values);
            privateIsDirty = false;
        }
        return privateCachedNodeList;
    }

    public bool PlaceNode(Vector3Int pos, GameObject prefab)
    {
        // 2. 생성 및 데이터 등록
        GameObject obj = Instantiate(prefab, transform);

        BasicNode newNode = obj.GetComponent<BasicNode>();

        var NodeOffset = GenerateBuildingOffsets(pos, newNode.NodeSize);

        foreach(var offset in NodeOffset)
        {
            // 설치하려는 건물의 크기에 해당하는 모든 칸이 유효한지 확인
            if (FloorManager.Instance.GetFloorAt(offset) == null || HasNodeAt(offset))
            {
                // 하나라도 겹치거나 바닥이 없으면 설치 실패 처리
                Destroy(obj);
                return false;
            }
        }

        foreach (var offset in NodeOffset)
        {
            // 모든 칸이 유효하므로, 각 칸마다 노드 등록
             if (!privateAllNodes.ContainsKey(offset))
             {
                 privateAllNodes.Add(offset, newNode);
            }
        }

        newNode.Setup(pos);

        obj.transform.position = GetCenterWorldPosition(NodeOffset);
        obj.transform.position += (baseAnchor + new Vector3(0, (newNode.NodeSize * 2 + 1) * 0.25f, 0));

        // 3. 만약 설치한 것이 울타리라면, 해당 칸을 동물 구역으로 설정
        if (newNode is FenceNode)
        {
            FloorManager.Instance.UpdateAnimalArea(pos, true);
        }

        // 고정 건물 크기로 변경으로 인해 안씀
        //NotifyNeighbors(pos);

        privateIsDirty = true;

        return true;
    }

    public void RemoveNode(Vector3Int cellPos)
    {
        if (privateAllNodes.ContainsKey(cellPos))
        {
            privateAllNodes.Remove(cellPos);

            // 데이터가 삭제되었으므로 리스트가 '더러워짐(Dirty)'을 표시
            privateIsDirty = true;
        }
    }

    private void NotifyNeighbors(Vector3Int pos)
    {
        Vector3Int[] neighbors = {
            pos + Vector3Int.up, pos + Vector3Int.down,
            pos + Vector3Int.left, pos + Vector3Int.right
        };

        foreach (var nPos in neighbors)
        {
            if (privateAllNodes.TryGetValue(nPos, out BasicNode node))
            {
                node.UpdateVisual();
            }
        }
    }

    public void TryFinalizeAllNewConnections(List<Vector3Int> newPlacedPositions)
    {
        #region [ 울타리 자유 설정일 경우 코드 ]
        // 이미 검사 완료된 좌표를 저장 (중복 방지용)
        HashSet<Vector3Int> processedPositions = new HashSet<Vector3Int>();

        foreach (Vector3Int startPos in newPlacedPositions)
        {
            // 이미 다른 덩어리 검사 때 확인했거나, 그새 지워진 노드라면 패스
            if (processedPositions.Contains(startPos) || !privateAllNodes.ContainsKey(startPos))
                continue;

            if (!(privateAllNodes[startPos] is IConnectable connectable))
                continue;

            // --- 여기서부터는 기존 BFS 로직과 동일 ---
            List<IConnectable> members = new List<IConnectable>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            HashSet<Vector3Int> visitedInThisGroup = new HashSet<Vector3Int>();
            System.Type targetType = privateAllNodes[startPos].GetType();

            queue.Enqueue(startPos);
            visitedInThisGroup.Add(startPos);

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();
                if (privateAllNodes.TryGetValue(current, out BasicNode node) && node.GetType() == targetType)
                {
                    members.Add((IConnectable)node);
                    processedPositions.Add(current); // 전체 검사 목록에서 제외하기 위해 기록

                    foreach (var dir in privateDirections)
                    {
                        Vector3Int next = current + dir;
                        if (!visitedInThisGroup.Contains(next) && privateAllNodes.ContainsKey(next))
                        {
                            visitedInThisGroup.Add(next);
                            queue.Enqueue(next);
                        }
                    }
                }
            }

            // --- 그룹 검증 ---
            if (members.Count < connectable.MinConnectionCount)
            {
                // [실패] 개수 부족: 모두 삭제
                foreach (var m in members)
                {
                    BasicNode node = (BasicNode)m;
                    privateAllNodes.Remove(node.GetCellPos());
                    privateIsDirty = true; // 삭제되었으니 캐시 갱신 필요
                    m.OnConnectionFailed();
                }
            }
            else
            {
                // [성공] 개수 충족: 새 그룹 생성 및 모든 멤버에게 그룹 참조 할당
                NodeGroup newGroup = new NodeGroup();

                foreach (var m in members)
                {
                    newGroup.AddMember(m); // 그룹에 멤버 추가

                    // 중요: 노드에게 자기가 어느 그룹 소속인지 알려줌
                    if (m is BasicNode node)
                    {
                        node.ParentGroup = newGroup;
                    }

                    m.OnConnectionSuccess(members.Count);
                }
            }
        }
        #endregion
    }

    public Vector3Int FindNearestEmptyCell(Vector3Int startPos)
    {
        //최대 탐색 거리
        int privateMaxRange = 20;

        for (int range = 1; range <= privateMaxRange; range++)
        {
            // 현재 range(반지름) 거리의 사각형 테두리를 순회합니다.
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    // 테두리 부분만 검사 (이미 이전 range에서 검사한 안쪽은 제외)
                    if (Mathf.Abs(x) != range && Mathf.Abs(y) != range) continue;

                    Vector3Int checkPos = new Vector3Int(startPos.x + x, startPos.y + y, 0);

                    // 1. 건물이 없는지 확인
                    // 2. 바닥 데이터가 존재하는지 확인
                    if (!HasNodeAt(checkPos) && FloorManager.Instance.GetFloorAt(checkPos) != null)
                    {
                        return checkPos;
                    }
                }
            }
        }

        // 만약 maxRange 안에서도 못 찾았다면 (매우 드문 경우) 원래 위치 반환
        return startPos;
    }

    public List<Vector3Int> GenerateBuildingOffsets(Vector3Int centerPos, int range)
    {
        List<Vector3Int> occupiedList = new List<Vector3Int>();

        // 예외 처리: 범위가 1 미만이면 기준점만 반환하거나 빈 리스트 반환
        if (range < 1)
        {
            occupiedList.Add(centerPos);
            return occupiedList;
        }

        // 규칙에 따른 X, Y의 최소/최대 오프셋 계산
        int minOffset = 0;
        int maxOffset = 0;

        if (range % 2 != 0)
        {
            // 홀수 (1, 3, 5...) 일 때는 기준점 중심으로 양방향 균등 확장
            minOffset = -(range / 2); // 1->0, 3->-1, 5->-2
            maxOffset = range / 2;    // 1->0, 3->1, 5->2
        }
        else
        {
            // 짝수 (2, 4...) 일 때는 위쪽(양수) 영역으로 한 칸 더 치우치게 확장
            minOffset = -(range / 2) + 1; // 2->0, 4->-1
            maxOffset = range / 2;        // 2->1, 4->2
        }

        // 계산된 범위를 바탕으로 이중 루프를 돌며 좌표 추가
        for (int x = minOffset; x <= maxOffset; x++)
        {
            for (int y = minOffset; y <= maxOffset; y++)
            {
                // 기준점에 상대 오프셋을 더해 최종 좌표 생성
                Vector3Int targetTile = new Vector3Int(centerPos.x + x, centerPos.y + y, centerPos.z);
                occupiedList.Add(targetTile);
            }
        }

        return occupiedList;
    }

    public Vector3 GetCenterWorldPosition(List<Vector3Int> occupiedTiles)
    {
        // 예외 처리: 리스트가 비어있으면 원점 반환
        if (occupiedTiles == null || occupiedTiles.Count == 0)
            return Vector3.zero;

        List<Vector3> worldPositions = new List<Vector3>();

        foreach(var tile in occupiedTiles)
        {
            Vector3 worldPos = privateTargetTilemap.CellToWorld(tile);
            worldPositions.Add(worldPos);
        }

        float averageX = 0f;
        float averageY = 0f;

        foreach(var pos in worldPositions)
        {
            averageX += pos.x;
            averageY += pos.y;
        }

        averageX /= worldPositions.Count;
        averageY /= worldPositions.Count;

        Vector3 centerWorldPos = new Vector3(averageX, averageY, 0f);

        return centerWorldPos;
    }
}