using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildController : BuildBase
{
    [SerializeField] private GameObject buildButtonPanel; // 빌드 모드 UI 패널
    [SerializeField] private GameObject buildCloseButton; // 빌드 모드 UI 패널

    [SerializeField] private BuildListBtnMaker buildListMaker;

    private List<Vector3Int> privateCurrentSessionPositions = new List<Vector3Int>();

    private void Start()
    {
        StartSetting();

        buildButtonPanel.SetActive(false);
        buildCloseButton.SetActive(false);
    }

    // UI 버튼에서 호출할 함수 (오브젝트 정보를 넘겨줌)
    public void SelectBuilding(GameObject buildingPrefab)
    {
        buildCloseButton.SetActive(true);

        UIOpenRegistry.RegisterUI();

        // 1. 카메라 줌 아웃
        privateMainCamera.orthographicSize = privateBuildModeZoom;

        CameraController.Instance.SetFreeMode(true);

        // 2. 캐릭터 숨기기
        SetCharactersActive(false);

        // 3. 건물 건설 시작
        privateSelectedPrefab = buildingPrefab;
        privateIsBuildMode = true;

        CreateGhost();
    }

    public override void HandleBuildInput()
    {
        var nodeInfo = privateSelectedPrefab.GetComponent<BasicNode>();
        var unlockableItem = privateSelectedPrefab.GetComponent<ShopUnlockableItem>();

        if (unlockableItem != null)
        {
            if(BuildableCountManager.Instance.GetBuildableCount(nodeInfo.NodeId) <= 0)
            {
                Debug.Log("설치 가능한 건물 수량이 부족합니다.");
                return;
            }
        }

        // 현재 고스트 오브젝트의 위치를 기준으로 설치
        Vector3 WorldPos = currentGhostObject.transform.position;
        Vector3Int cellPos = privateBuildManager.PrivateTargetTilemap.WorldToCell(WorldPos);

        // BuildManager에 설치 요청 (중복 검사는 BuildManager 내부에서 수행됨)
        bool isResult = privateBuildManager.PlaceNewNode(cellPos, privateSelectedPrefab);

        if(isResult)
        {
            if (!privateCurrentSessionPositions.Contains(cellPos))
                privateCurrentSessionPositions.Add(cellPos);

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

        buildButtonPanel.SetActive(false);
        buildCloseButton.SetActive(false);

        buildListMaker.SetBuildButton();

        UIOpenRegistry.UnregisterUI();
    }

    public void OpenBuildButtonUI()
    {
        buildButtonPanel.SetActive(!buildButtonPanel.activeSelf);
    }
}