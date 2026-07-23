using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinder
{
    private class Node
    {
        public Vector3Int cellPos;
        public Node parent;
        public int gCost; // 시작점부터의 거리
        public int hCost; // 목적지까지의 추정 거리 (휴리스틱)
        public int FCost => gCost + hCost;

        public Node(Vector3Int cellPos, Node parent, int gCost, int hCost)
        {
            this.cellPos = cellPos;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }

    private static readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),   // 상
        new Vector3Int(0, -1, 0),  // 하
        new Vector3Int(-1, 0, 0),  // 좌
        new Vector3Int(1, 0, 0)    // 우
    };

    /// <summary>
    /// startCell에서 targetCell까지 Road를 타고 가는 최단 셀 좌표 리스트를 반환합니다.
    /// </summary>
    public static List<Vector3Int> FindPath(Tilemap grid, Vector3Int startCell, Vector3Int targetCell)
    {
        List<Node> openList = new List<Node>();
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();

        Node startNode = new Node(startCell, null, 0, GetDistance(startCell, targetCell));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F 비용이 가장 적은 노드 선택
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost ||
                   (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.cellPos);

            // 목적지 도착 시 경로 복원
            if (currentNode.cellPos == targetCell)
            {
                return RetracePath(startNode, currentNode);
            }

            // 4방향 이웃 탐색
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighborCell = currentNode.cellPos + dir;

                if (closedList.Contains(neighborCell)) continue;

                // Road가 있는 타일인지 검사
                Vector3 neighborWorldPos = grid.GetCellCenterWorld(neighborCell);
                if (BuildManager.Instance.GetRoadAt(neighborWorldPos) == null) continue;

                int newCostToNeighbor = currentNode.gCost + 1;
                Node neighborNode = openList.Find(n => n.cellPos == neighborCell);

                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborCell, currentNode, newCostToNeighbor, GetDistance(neighborCell, targetCell));
                    openList.Add(neighborNode);
                }
                else if (newCostToNeighbor < neighborNode.gCost)
                {
                    neighborNode.gCost = newCostToNeighbor;
                    neighborNode.parent = currentNode;
                }
            }
        }

        return null; // 경로를 찾지 못함
    }

    private static List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node curr = endNode;

        while (curr != startNode)
        {
            path.Add(curr.cellPos);
            curr = curr.parent;
        }

        path.Reverse(); // 목적지 -> 시작점 순서를 역순으로 정렬
        return path;
    }

    // 맨해튼 거리 계산법 (아이소메트릭 그리드용)
    private static int GetDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}