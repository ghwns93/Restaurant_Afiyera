using UnityEngine;

public abstract class BasicNode : MonoBehaviour
{
    public int NodeId;
    public string NodeName;
    public Sprite NodeMarkSprite;

    public bool IsOverlapable = false; // 건물 npc랑 유저 겹치기 가능 여부
    public bool IsBuildable = true; // 건물 설치 가능 여부

    // 자식 클래스에서도 접근 가능하도록 protected 사용
    protected Vector3Int privateCellPos;

    [SerializeField] protected ItemData harvestItem; // 수확 시 얻는 아이템 정보
    [SerializeField] protected int harvestAmount = 1; // 수확 시 얻는 아이템 수량
    [SerializeField] public int harvestTime = 3; // 수확까지 걸리는 시간 (일 단위)

    protected int currentDayCount = 0; // 현재 일 수 카운트

    [SerializeField] protected int privateDayCount = 1; // 몇 일마다 실행할지 (주기)
    [SerializeField] private int nodeSize = 1; // 건물 사이즈

    protected BasicNpcScript nodesBasicNpcScript;

    public int DayCount => privateDayCount;
    public int NodeSize { get => nodeSize; set => nodeSize = value; }

    public NodeGroup ParentGroup { get; set; }

    public virtual void Setup(BuildingData data)
    {
        privateCellPos = data.position;

        transform.localScale = new Vector3(NodeSize, NodeSize, 1);

        //if(NodeSize % 2 == 0)
        //{
        //    transform.position += new Vector3(0.5f, 0.5f, 0); // 짝수 사이즈는 중앙이 격자선에 오도록 보정
        //}

        var orderLayer = GetComponent<GroupSorting>();

        if (orderLayer != null)
        {
            orderLayer.SetFenceOrder(privateCellPos);
        }

        //Debug.Log("설치시 남은 수확시간 : " + data.remainHarvestTime);

        currentDayCount = data.remainHarvestTime;

        nodesBasicNpcScript = GetComponent<BasicNpcScript>();
    }

    public virtual void SaveBuildData()
    {
        var data = new BuildingData
        {
            id = NodeId,
            position = privateCellPos,
            remainHarvestTime = currentDayCount
        };

        //Debug.Log("저장되는 시간 : " + currentDayCount);

        BuildLoadManager.Instance.NewDataStructure(data);
    }

    // 외부에서 좌표 정보를 확인할 때 사용
    public Vector3Int GetCellPos()
    {
        return privateCellPos;
    }

    // 건물마다 시각적 업데이트 로직이 다를 수 있으므로 가상 함수로 선언
    public abstract void UpdateVisual();

    // 각 노드가 수행할 구체적인 행동 (자식 클래스에서 구현)

    //수확 행동
    public abstract void HarvestAction();
    //관리 수량증가 행동
    public abstract void ManagementCountAction();
    //관리 주기감소 행동
    public abstract void ManagementCycleAction();
    //하루 종료 시 행동
    public abstract void DayAction();
}
