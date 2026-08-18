using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class DragMove : MonoBehaviour
{
    private Tilemap targetTilemap;
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    [Header("드래그 허용 레이어")]
    [SerializeField] private LayerMask targetLayer;

    public void Init(Tilemap tilemap, LayerMask layer)
    {
        targetTilemap = tilemap;
        mainCamera = Camera.main;
        targetLayer = layer;
    }

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        // UI를 클릭 중일 때는 드래그 시작 방지
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 1. 마우스 왼쪽 버튼을 누른 순간 (클릭 시작)
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // 2. 마우스 왼쪽 버튼을 누르고 있는 동안 (드래그 중)
        if (isDragging && Input.GetMouseButton(0))
        {
            PerformDrag();
        }

        // 3. 마우스 왼쪽 버튼을 뗀 순간 (드래그 종료)
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    private void TryStartDrag()
    {
        if (targetTilemap == null || mainCamera == null) return;

        Vector2 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, targetLayer);

        // 클릭한 위치에 내 콜라이더가 존재하면 드래그 시작
        if (hitCollider != null && hitCollider.transform == transform)
        {
            isDragging = true;
            // 클릭 지점과 오브젝트 중심 간의 오프셋 계산
            offset = transform.position - (Vector3)mouseWorldPos;
        }
    }

    private void PerformDrag()
    {
        if (targetTilemap == null) return;

        Vector3 rawWorldPos = GetMouseWorldPosition() + offset;
        Vector3Int cellPosition = targetTilemap.WorldToCell(rawWorldPos);
        Vector3 snappedWorldPos = targetTilemap.GetCellCenterWorld(cellPosition);

        // Z축 보정
        snappedWorldPos.z = 0;
        transform.position = snappedWorldPos;
    }

    private void StopDrag()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}