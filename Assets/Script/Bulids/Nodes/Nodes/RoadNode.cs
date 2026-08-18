using UnityEngine;

public class RoadNode : BasicNode, EditorInterface
{
    private void Start()
    {
        if(BuildManager.Instance != null)
        {
            Vector3Int cellPos = BuildManager.Instance.PrivateTargetTilemap.WorldToCell(transform.position);
            var data = new BuildingData
            {
                id = NodeId,
                position = cellPos,
                remainHarvestTime = harvestTime
            };
            BuildManager.Instance.InsertNode(data, gameObject);
        }
    }

    public override void DayAction(){}

    public override void HarvestAction(){}

    public override void ManagementCountAction(){}

    public override void ManagementCycleAction(){}

    public override void UpdateVisual(){}

    public GameObject GetEditorPrefab()
    {
        return this.gameObject;
    }
}
