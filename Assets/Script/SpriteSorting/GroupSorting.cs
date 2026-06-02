using UnityEngine;
using UnityEngine.Rendering;

public class GroupSorting : MonoBehaviour
{
    private SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    // 울타리가 배치될 때 이 함수를 실행해줍니다.
    public void SetFenceOrder(Vector3Int cellPos)
    {
        // X와 Y 좌표의 합을 음수화하고, NPC가 소수점(사이사이)으로 지나갈 것을 대비해 100을 곱합니다.
        // 예: (0,0) -> 0 // (1,0) -> -100 // (-1, -1) -> 200
        int calculatedOrder = -(cellPos.x + cellPos.y) * 100;

        sortingGroup.sortingOrder = calculatedOrder;
    }
}
