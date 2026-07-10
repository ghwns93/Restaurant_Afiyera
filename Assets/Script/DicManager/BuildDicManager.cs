using UnityEngine;

public class BuildDicManager : BaseDicManager<int, GameObject>
{
    protected override int GetKey(GameObject data)
    {
        var nodeComponent = data.GetComponent<BasicNode>();
        if (nodeComponent != null)
        {
            return nodeComponent.NodeId;
        }
        else
        {
            Debug.LogWarning($"GameObject {data.name}에 BasicNode 컴포넌트가 없습니다.");
            return -1; // 유효하지 않은 키 반환
        }
    }
}
