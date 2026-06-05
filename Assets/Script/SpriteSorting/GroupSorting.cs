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
        int calculatedOrder = -(cellPos.x + cellPos.y) * 100 - 150;

        sortingGroup.sortingOrder = calculatedOrder;
    }
}
