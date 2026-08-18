using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BuildBase : MonoBehaviour
{
    protected GameObject privateSelectedPrefab; // 현재 선택된 건물 프리팹
    protected bool privateIsBuildMode = false;

    [SerializeField] protected Camera privateMainCamera;
    [SerializeField] protected BuildManager privateBuildManager;

    // 카메라 설정
    protected float privateOriginalZoom;
    [SerializeField] protected float privateBuildModeZoom = 10f;

    // 캐릭터 관리
    protected GameObject privatePlayer;
    protected GameObject[] privateNPCs;

    [Header("[고스트 관련]")]
    [SerializeField] protected Sprite arrowSprite;   // 화살표 이미지 (위쪽을 바라보는 이미지)
    [SerializeField] protected Sprite rotateSprite;  // 회전 버튼 이미지
    [SerializeField] protected Sprite cancelSprite;  // X 버튼 이미지
    [SerializeField] protected LayerMask ghostLayer; // 인스펙터에서 "Ghost" 레이어 지정
    [SerializeField] protected GameObject currentGhostObject; // 빌드 모드에서 보여줄 유령 건물 프리팹
    private void Awake()
    {
        // 인스펙터에서 연결 안 했을 경우를 대비한 자동 할당
        if (privateBuildManager == null)
        {
            privateBuildManager = BuildManager.Instance;
        }
    }

    protected void StartSetting()
    {
        privatePlayer = GameObject.FindGameObjectWithTag("Player");
        privateOriginalZoom = privateMainCamera.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && privateIsBuildMode)
        {
            CancelBuildMode();
        }
    }

    protected void SetCharactersActive(bool isActive)
    {
        privatePlayer.SetActive(isActive);

        if (privateNPCs == null || privateNPCs.Length == 0) privateNPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in privateNPCs) npc.SetActive(isActive);
    }

    public void CreateGhost()
    {
        if (currentGhostObject != null)
        {
            Destroy(currentGhostObject);
        }

        currentGhostObject = Instantiate(privateSelectedPrefab);
        currentGhostObject.name = privateSelectedPrefab.name + "_Ghost";
        currentGhostObject.layer = LayerMask.NameToLayer("Ghost");

        SceneManager.MoveGameObjectToScene(currentGhostObject, gameObject.scene);

        BasicNode bn = currentGhostObject.GetComponent<BasicNode>();
        if (bn != null)
        {
            currentGhostObject.transform.localScale = new Vector3(bn.NodeSize, bn.NodeSize, 1f);
        }

        MonoBehaviour[] scripts = currentGhostObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour script in scripts)
        {
            if (script is BasicNode) continue;

            // 고스트가 생성되자마자 Awake/Start 등이 실행되는 것을 방지하기 위해 Immediate로 즉시 제거
            DestroyImmediate(script);
        }

        Collider[] colliders = currentGhostObject.GetComponentsInChildren<Collider>(true);
        foreach (Collider coll in colliders)
        {
            coll.enabled = false;
        }

        SpriteRenderer[] sprites = currentGhostObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sprite in sprites)
        {
            Color color = sprite.color;
            color.a = 0.5f; // 0.0f (완전 투명) ~ 1.0f (불투명). 0.5f면 50% 투명
            sprite.color = color;
        }

        SpriteRenderer[] allSprites = currentGhostObject.GetComponentsInChildren<SpriteRenderer>(true);

        if (allSprites.Length > 0)
        {
            // A. 모든 자식 스프라이트 영역을 합쳐서 전체 바운딩 박스 생성
            Bounds combinedBounds = allSprites[0].bounds;
            for (int i = 1; i < allSprites.Length; i++)
            {
                combinedBounds.Encapsulate(allSprites[i].bounds);
            }

            // B. BoxCollider2D 추가
            BoxCollider2D boxCol = currentGhostObject.GetComponent<BoxCollider2D>();

            if (boxCol == null)
            {
                boxCol = currentGhostObject.AddComponent<BoxCollider2D>();
            }

            // C. 고스트의 스케일 영향을 감안한 로컬 Size 및 Offset 계산
            // (combinedBounds는 월드 기준 크기이므로, 고스트의 lossyScale로 나누어 로컬 크기 맞춤)
            Vector3 ghostScale = currentGhostObject.transform.lossyScale;

            float scaleX = ghostScale.x != 0 ? ghostScale.x : 1f;
            float scaleY = ghostScale.y != 0 ? ghostScale.y : 1f;

            // 건물 크기(Bounds Size)에 딱 들어맞는 Collider 크기 지정
            boxCol.size = new Vector2(
                combinedBounds.size.x / scaleX,
                combinedBounds.size.y / scaleY
            );

            // 건물의 중심점(Bounds Center) 위치에 맞게 Collider 중심 오프셋 지정
            Vector3 localCenter = currentGhostObject.transform.InverseTransformPoint(combinedBounds.center);
            boxCol.offset = new Vector2(localCenter.x, localCenter.y);
        }

        DragMove dragScript = currentGhostObject.AddComponent<DragMove>();
        dragScript.Init(privateBuildManager.PrivateTargetTilemap, ghostLayer);

        float spawnDistance = 10f;

        // 4. 카메라 중앙 위치를 타일맵 칸에 맞추어 배치
        if (Camera.main != null && privateBuildManager.PrivateTargetTilemap != null)
        {
            // A. 카메라 중앙의 가상 월드 좌표 구하기
            Vector3 rawCameraCenterWorld = Camera.main.transform.position + (Camera.main.transform.forward * spawnDistance);

            // B. 월드 좌표를 타일맵의 셀 좌표(int, int, int)로 변환
            Vector3Int cellPosition = privateBuildManager.PrivateTargetTilemap.WorldToCell(rawCameraCenterWorld);

            // C. 셀 좌표의 "정중앙 월드 좌표" 가져오기
            Vector3 snappedWorldPosition = privateBuildManager.PrivateTargetTilemap.GetCellCenterWorld(cellPosition);

            // D. 고스트를 격자에 맞춘 위치로 이동 (2D라면 Z축 고정 보정이 필요할 수 있음)
            currentGhostObject.transform.position = snappedWorldPosition;
        }

        AttachGhostUI(currentGhostObject);
    }

    private void AttachGhostUI(GameObject ghostObj)
    {
        // 1. UI 모아둘 부모 오브젝트 생성
        GameObject uiParent = new GameObject("Ghost_UI_Overlay");
        uiParent.transform.SetParent(ghostObj.transform);
        uiParent.transform.localPosition = Vector3.zero;

        // 2. 건물의 Scale 영향 무력화
        Vector3 ghostScale = ghostObj.transform.lossyScale;
        uiParent.transform.localScale = new Vector3(
            1f / (ghostScale.x != 0 ? ghostScale.x : 1f),
            1f / (ghostScale.y != 0 ? ghostScale.y : 1f),
            1f / (ghostScale.z != 0 ? ghostScale.z : 1f)
        );

        // 3. 자식에 있는 모든 SpriteRenderer 영역을 합쳐서 전체 건물 크기 계산
        SpriteRenderer[] renderers = ghostObj.GetComponentsInChildren<SpriteRenderer>();
        float sizeOffset = 1.0f;

        if (renderers.Length > 0)
        {
            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // 전체 합쳐진 이미지 크기를 바탕으로 오프셋 결정
            sizeOffset = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y) * 0.6f;
        }

        // 4. 아이소메트릭 대각선 4방향 화살표 배치
        float isoX = sizeOffset * 0.866f;
        float isoY = sizeOffset * 0.5f;

        // ↗ (북동)
        CreateSpriteObject("Arrow_NE", arrowSprite, uiParent.transform,
            new Vector3(isoX, isoY, 0), Quaternion.Euler(0, 0, -45f));

        // ↘ (남동)
        CreateSpriteObject("Arrow_SE", arrowSprite, uiParent.transform,
            new Vector3(isoX, -isoY, 0), Quaternion.Euler(0, 0, -135f));

        // ↙ (남서)
        CreateSpriteObject("Arrow_SW", arrowSprite, uiParent.transform,
            new Vector3(-isoX, -isoY, 0), Quaternion.Euler(0, 0, 135f));

        // ↖ (북서)
        CreateSpriteObject("Arrow_NW", arrowSprite, uiParent.transform,
            new Vector3(-isoX, isoY, 0), Quaternion.Euler(0, 0, 45f));

        // 5. 하단 버튼 배치
        float buttonYOffset = -isoY - 0.8f;
        float buttonXOffset = 0.4f;

        GameObject rotateBtnObj = CreateButtonObject("Btn_Rotate", rotateSprite, uiParent.transform, new Vector3(-buttonXOffset, buttonYOffset, 0));
        WorldButton rotateBtn = rotateBtnObj.GetComponent<WorldButton>();
        rotateBtn.onClick.AddListener(() => HandleBuildInput());

        GameObject btnCancelObject = CreateButtonObject("Btn_Cancel", cancelSprite, uiParent.transform, new Vector3(buttonXOffset, buttonYOffset, 0));
        WorldButton btnCancel = btnCancelObject.GetComponent<WorldButton>();
        btnCancel.onClick.AddListener(() => CancelBuildMode());

    }

    /// <summary>
    /// 단순 표시용 Sprite 오브젝트 생성 헬퍼 함수
    /// </summary>
    private GameObject CreateSpriteObject(string name, Sprite sprite, Transform parent, Vector3 localPos, Quaternion localRot)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = localRot;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 10; // 건물보다 위에 보이도록 소팅 오더 설정

        return obj;
    }

    /// <summary>
    /// 클릭 가능한 버튼 Sprite 오브젝트 생성 헬퍼 함수
    /// </summary>
    private GameObject CreateButtonObject(string name, Sprite sprite, Transform parent, Vector3 localPos)
    {
        GameObject btnObj = CreateSpriteObject(name, sprite, parent, localPos, Quaternion.identity);

        // 2D 마우스 클릭 감지를 위해 Collider2D 및 Button 컴포넌트 추가
        btnObj.AddComponent<BoxCollider2D>();

        // 2. UI Button 대신 WorldButton 스크립트 추가
        btnObj.AddComponent<WorldButton>();

        return btnObj;
    }

    protected void AdjustCharacterPositions()
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
        if (privateBuildManager.HasOverlapNodeAt(cellPos))
        {
            // 3. 가장 가까운 빈 공간 찾기 (주변 8칸 탐색)
            Vector3Int nearestEmptyCell = privateBuildManager.FindNearestEmptyCell(cellPos);

            // 4. 위치 튕기기 (이동)
            target.position = privateBuildManager.PrivateTargetTilemap.GetCellCenterWorld(nearestEmptyCell) + new Vector3(0f, characterHeight, 0f);
        }
    }

    public abstract void HandleBuildInput();
    public abstract void CancelBuildMode();
}
