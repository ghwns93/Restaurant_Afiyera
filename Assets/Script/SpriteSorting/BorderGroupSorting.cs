using UnityEngine;
using UnityEngine.Rendering;

public class BorderGroupSorting : MonoBehaviour
{
    private SortingGroup sortingGroup;

    private void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();

        Vector3 mousePos = transform.position;

        Vector3Int cellPos = BuildManager.Instance.PrivateTargetTilemap.WorldToCell(mousePos);

        SetFenceOrder(cellPos);
    }

    // 울타리가 배치될 때 이 함수를 실행해줍니다.
    private void SetFenceOrder(Vector3Int cellPos)
    {
        int calculatedOrder = -(cellPos.x + cellPos.y) * 100 - 150;

        sortingGroup.sortingOrder = calculatedOrder;
    }
}
