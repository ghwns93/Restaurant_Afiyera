using UnityEngine;

public abstract class BasicNode : MonoBehaviour
{
    // 자식 클래스에서도 접근 가능하도록 protected 사용
    protected Vector3Int privateCellPos;

    [SerializeField] protected int privateDayCount = 1; // 몇 일마다 실행할지 (주기)
    public int DayCount => privateDayCount;

    public NodeGroup ParentGroup { get; set; }

    public virtual void Setup(Vector3Int pos)
    {
        privateCellPos = pos;
    }

    // 외부에서 좌표 정보를 확인할 때 사용
    public Vector3Int GetCellPos()
    {
        return privateCellPos;
    }

    // 건물마다 시각적 업데이트 로직이 다를 수 있으므로 가상 함수로 선언
    public abstract void UpdateVisual();

    // 각 노드가 수행할 구체적인 행동 (자식 클래스에서 구현)
    public abstract void DayAction();
}
