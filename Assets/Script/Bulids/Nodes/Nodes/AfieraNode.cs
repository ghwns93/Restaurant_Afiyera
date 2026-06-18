using UnityEngine;

public class AfieraNode : BasicNode
{
    private void Start()
    {
        if(BuildManager.Instance != null)
        {
            Vector3Int cellPos = BuildManager.Instance.PrivateTargetTilemap.WorldToCell(transform.position);
            BuildManager.Instance.InsertNode(cellPos, gameObject);
        }
    }

    public override void DayAction()
    {

    }

    public override void HarvestAction()
    {
        SceneController.Instance.LoadSubScene(SceneType.Home);
    }

    public override void ManagementCountAction()
    {

    }

    public override void ManagementCycleAction()
    {

    }

    public override void UpdateVisual()
    {

    }
}
