using UnityEngine;

public class FenceNode : BasicNode, IConnectable, EditorInterface
{
    [SerializeField] private GameObject spriteLU; // 좌상
    [SerializeField] private GameObject spriteLD; // 좌하
    [SerializeField] private GameObject spriteRU; // 우상
    [SerializeField] private GameObject spriteRD; // 우하

    [SerializeField] private int privateMinCount = 3;
    public int MinConnectionCount => privateMinCount;

    public override void Setup(BuildingData bData)
    {
        base.Setup(bData);
        //UpdateVisual(); // 설치 시 초기 그래픽 업데이트
    }

    #region [ 가변 울타리 코드 ]
    // 주변 상태를 확인하고 내 그래픽을 갱신하는 핵심 함수
    // 26.05.28 고정크기로 변경
    public override void UpdateVisual()
    {
        // FenceManager에게 주변에 울타리가 있는지 물어봅니다.
        bool hasLeftUp = BuildManager.Instance.HasNodeAt<FenceNode>(privateCellPos + new Vector3Int(0, 1, 0));
        bool hasRightUp = BuildManager.Instance.HasNodeAt<FenceNode>(privateCellPos + new Vector3Int(1, 0, 0));
        bool hasLeftDown = BuildManager.Instance.HasNodeAt<FenceNode>(privateCellPos + new Vector3Int(-1, 0, 0));
        bool hasRightDown = BuildManager.Instance.HasNodeAt<FenceNode>(privateCellPos + new Vector3Int(0, -1, 0));

        // 예시 로직: 주변에 울타리가 없다면 해당 방향의 그래픽을 켭니다.
        // (기획하신 울타리 연결 모양에 따라 조건을 수정하세요)
        spriteLU.SetActive(!hasLeftUp);
        spriteRU.SetActive(!hasRightUp);
        spriteLD.SetActive(!hasLeftDown);
        spriteRD.SetActive(!hasRightDown);
    }

    public void OnConnectionFailed()
    {
        // 연결 부족 시 스스로 파괴 (BuildManager에서 처리해도 됨)
        //Debug.Log("연결 부족으로 철거됩니다.");
        //Destroy(gameObject);
    }

    public void OnConnectionSuccess(int totalCount)
    {
        //Debug.Log($"{totalCount}개의 울타리가 연결되었습니다!");
        // 여기서 울타리 비주얼 연결 등의 추가 로직 수행
    }
    #endregion

    public override void HarvestAction()
    {
        //상호작용 방식 변경
        //if(isHarvested == true)
        //{               
        //    NpcTalkUIManager.Instance.SetTalkText("이미 수확된 울타리입니다.");
        //    return;
        //}
        //isHarvested = true;

        InventoryManager.Instance.AddItem(harvestItem, harvestAmount);
        currentDayCount = 1;

        SaveBuildData();

        NpcTalkUIManager.Instance.EndTalk();
    }

    public override void DayAction()
    {
        var npcScript = GetComponent<BasicNpcScript>();

        Debug.Log($"{NodeName} 일 수 카운트 증가! 현재 일 수: {currentDayCount}, 수확까지 남은 일 수: {harvestTime - currentDayCount}");

        if (currentDayCount == harvestTime)
        {
            Debug.Log($"{NodeName} 수확가능!");

            npcScript.ResetNpcInteraction(QuestInteractionType.Harvest);
        }
        else
        {
            currentDayCount = Mathf.Min(currentDayCount + 1, harvestTime); // 일 수 카운트 증가 (harvestTime 이상으로는 증가하지 않음)
        }

        npcScript.ResetNpcInteraction(QuestInteractionType.Management);

        SaveBuildData();
    }

    public override void ManagementCountAction()
    {
        Debug.Log("울타리 관리 수량 증가!");
        NpcTalkUIManager.Instance.EndTalk();
    }

    public override void ManagementCycleAction()
    {
        Debug.Log("울타리 주기 감소 증가!");
        NpcTalkUIManager.Instance.EndTalk();
    }

    public GameObject GetEditorPrefab()
    {
        return this.gameObject;
    }
}
