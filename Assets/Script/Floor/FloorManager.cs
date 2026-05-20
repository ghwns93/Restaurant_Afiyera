using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [SerializeField] private Tilemap privateFloorTilemap;
    private Dictionary<Vector3Int, FloorNode> privateFloorDict = new Dictionary<Vector3Int, FloorNode>();

    private void Awake()
    {
        Instance = this;
        InitializeFloors();
    }

    private void InitializeFloors()
    {
        BoundsInt bounds = privateFloorTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (privateFloorTilemap.HasTile(pos))
            {
                privateFloorDict.Add(pos, new FloorNode(pos));
            }
        }
    }

    // 특정 위치의 바닥 데이터를 가져오는 함수
    public FloorNode GetFloorAt(Vector3Int pos)
    {
        if (privateFloorDict.TryGetValue(pos, out FloorNode floor))
        {
            return floor;
        }

        return null;
    }

    // 울타리 설치 시 해당 칸의 동물 구역 속성을 변경
    public void UpdateAnimalArea(Vector3Int pos, bool isInside)
    {
        FloorNode floor = GetFloorAt(pos);
        if (floor != null)
        {
            floor.PrivateIsAnimalArea = isInside;
        }
    }
}