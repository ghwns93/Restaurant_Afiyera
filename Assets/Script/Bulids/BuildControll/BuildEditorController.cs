using UnityEngine;
using UnityEngine.EventSystems;

public class BuildEditorController : BuildBase
{
    [Header("[에디터 관련]")]
    [SerializeField] private LayerMask nodeLayer; // 선택 사항: Node 오브젝트 전용 레이어 지정

    private void Start()
    {
        StartSetting();
    }

    private void Update()
    {
        if(!privateIsBuildMode || privateSelectedPrefab != null) return;

        if(Input.GetMouseButtonDown(0))
        {
            // UI 클릭 중일 때는 레이캐스트 방지
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            EditorInterface clickedNode = GetClickedNode();

            if (clickedNode != null)
            {
                privateSelectedPrefab = clickedNode.GetEditorPrefab();

                CreateGhost();

                currentGhostObject.transform.position = privateSelectedPrefab.transform.position;
            }
        }
    }

    private EditorInterface GetClickedNode()
    {
        // 1. 마우스 화면 좌표를 월드 좌표로 변환
        Vector2 mouseWorldPos = privateMainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. 해당 지점에 위치한 2D 콜라이더 탐색 (레이어가 설정되어 있으면 특정 레이어만 탐색)
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, nodeLayer);

        // 3. 콜라이더가 맞았고, 그 오브젝트(또는 부모)에 BaseNode 컴포넌트가 있는지 확인
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<EditorInterface>();
        }

        return null;
    }

    public void EnterBuildMode()
    {
        UIOpenRegistry.RegisterUI();

        // 1. 카메라 줌 아웃
        privateMainCamera.orthographicSize = privateBuildModeZoom;

        CameraController.Instance.SetFreeMode(true);

        // 2. 캐릭터 숨기기
        SetCharactersActive(false);

        privateIsBuildMode = true;
    }

    public override void HandleBuildInput()
    {
        // 현재 고스트 오브젝트의 위치를 기준으로 설치
        Vector3 WorldPos = currentGhostObject.transform.position;
        Vector3Int cellPos = privateBuildManager.PrivateTargetTilemap.WorldToCell(WorldPos);

        BasicNode bn = privateSelectedPrefab.GetComponent<BasicNode>();

        bool result = BuildManager.Instance.RelocateNode(bn, cellPos);

        if (result)
        {
            privateSelectedPrefab.transform.position = WorldPos;

            GroupSorting gs = privateSelectedPrefab.GetComponent<GroupSorting>();

            gs.SetFenceOrder(cellPos);

            CancelBuildMode();
        }
    }

    public override void CancelBuildMode()
    {
        if (currentGhostObject != null)
        {
            Destroy(currentGhostObject);
            currentGhostObject = null;
        }

        CameraController.Instance.SetFreeMode(false);

        // 1. 겹침 확인 및 캐릭터 위치 조정 (가장 중요)
        SetCharactersActive(true);
        AdjustCharacterPositions();

        // 2. 원래대로 복구
        privateMainCamera.orthographicSize = privateOriginalZoom;
        privateIsBuildMode = false;
        privateSelectedPrefab = null;

        UIOpenRegistry.UnregisterUI();
    }
}
