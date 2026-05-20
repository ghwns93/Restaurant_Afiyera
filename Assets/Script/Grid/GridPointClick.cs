using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPointClick : MonoBehaviour
{
    [SerializeField] private Tilemap privateTargetTilemap;

    // Update is called once per frame
    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 실행
        if (Input.GetMouseButtonDown(0))
        {
            SetPositionToTileCenter();
        }
    }

    private void SetPositionToTileCenter()
    {
        if (privateTargetTilemap == null) return;

        // 1. 마우스 클릭 위치를 셀 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickCellPos = privateTargetTilemap.WorldToCell(mouseWorldPos);

        // 2. 셀 좌표를 해당 타일의 '중앙' 월드 좌표로 변환
        // 이 함수는 그리드의 Cell Size(스케일)가 바뀌어도 자동으로 계산된 중심점을 줍니다.
        Vector3 centerWorldPos = privateTargetTilemap.GetCellCenterWorld(clickCellPos);

        Debug.Log($"셀 좌표: {clickCellPos} -> 월드 중심 좌표: {centerWorldPos}");
    }
}
