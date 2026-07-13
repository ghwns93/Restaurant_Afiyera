using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildController : MonoBehaviour
{
    [SerializeField] private BuildManager privateBuildManager;
    [SerializeField] private Camera privateMainCamera;

    private GameObject privateSelectedPrefab; // 현재 선택된 건물 프리팹

    // 카메라 설정
    private float privateOriginalZoom;
    [SerializeField] private float privateBuildModeZoom = 10f;

    // 캐릭터 관리
    private GameObject privatePlayer;
    private GameObject[] privateNPCs;
    private bool privateIsBuildMode = false;

    private List<Vector3Int> privateCurrentSessionPositions = new List<Vector3Int>();

    private void Awake()
    {
        // 인스펙터에서 연결 안 했을 경우를 대비한 자동 할당
        if (privateBuildManager == null)
        {
            privateBuildManager = BuildManager.Instance;
        }
    }

    private void Start()
    {
        privatePlayer = GameObject.FindGameObjectWithTag("Player");
        privateOriginalZoom = privateMainCamera.orthographicSize;
    }

    private void Update()
    {
        // 1. 취소 로직 (ESC)
        if (Input.GetKeyDown(KeyCode.Escape) && privateIsBuildMode)
        {
            CancelBuildMode();
        }

        if (!privateIsBuildMode || privateSelectedPrefab == null) return;

        // 2. 마우스 입력 처리 (클릭 및 드래그 대응을 위해 GetMouseButton 사용)
        if (Input.GetMouseButtonDown(0))
        {
            HandleBuildInput();
        }
    }

    // UI 버튼에서 호출할 함수 (오브젝트 정보를 넘겨줌)
    public void SelectBuilding(GameObject buildingPrefab)
    {
        UIOpenRegistry.RegisterUI();

        privateIsBuildMode = true;

        // 1. 카메라 줌 아웃
        privateMainCamera.orthographicSize = privateBuildModeZoom;

        CameraController.Instance.SetFreeMode(true);

        // 2. 캐릭터 숨기기
        SetCharactersActive(false);

        // 3. 건물 건설 시작
        privateSelectedPrefab = buildingPrefab;
        privateIsBuildMode = true;
    }

    private void HandleBuildInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPos = privateBuildManager.PrivateTargetTilemap.WorldToCell(mouseWorldPos);

        // BuildManager에 설치 요청 (중복 검사는 BuildManager 내부에서 수행됨)
        bool isResult = privateBuildManager.PlaceNewNode(cellPos, privateSelectedPrefab);

        if(isResult)
        {
            if (!privateCurrentSessionPositions.Contains(cellPos))
                privateCurrentSessionPositions.Add(cellPos);
        }
    }

    private void CancelBuildMode()
    {
        CameraController.Instance.SetFreeMode(false);

        //privateBuildManager.TryFinalizeAllNewConnections(privateCurrentSessionPositions);

        // 1. 겹침 확인 및 캐릭터 위치 조정 (가장 중요)
        SetCharactersActive(true);
        AdjustCharacterPositions();

        // 2. 원래대로 복구
        privateMainCamera.orthographicSize = privateOriginalZoom;
        privateIsBuildMode = false;
        privateSelectedPrefab = null;

        UIOpenRegistry.UnregisterUI();
    }

    private void SetCharactersActive(bool isActive)
    {
        privatePlayer.SetActive(isActive);

        if(privateNPCs == null || privateNPCs.Length == 0) privateNPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in privateNPCs) npc.SetActive(isActive);
    }

    private void AdjustCharacterPositions()
    {
        // 플레이어 체크
        CheckAndRepel(privatePlayer.transform);

        // 모든 NPC 체크
        foreach (var npc in privateNPCs)
        {
            CheckAndRepel(npc.transform);
        }
    }

    private void CheckAndRepel(Transform target)
    {
        // 1. 현재 캐릭터가 있는 셀 좌표 확인
        float characterHeight = target.localScale.y / 2; // 캐릭터의 높이 (스케일 기준)
        Vector3 pivot = target.position - new Vector3(0f, characterHeight, 0f); // 캐릭터의 발 위치를 기준으로 계산
        Vector3Int cellPos = privateBuildManager.PrivateTargetTilemap.WorldToCell(pivot);

        // 2. 해당 위치에 건물이 있는지 확인
        if (privateBuildManager.HasNodeAt(cellPos))
        {
            // 3. 가장 가까운 빈 공간 찾기 (주변 8칸 탐색)
            Vector3Int nearestEmptyCell = privateBuildManager.FindNearestEmptyCell(cellPos);

            // 4. 위치 튕기기 (이동)
            target.position = privateBuildManager.PrivateTargetTilemap.GetCellCenterWorld(nearestEmptyCell) + new Vector3(0f, characterHeight, 0f);
        }
    }
}