using UnityEngine;

public class AfieraNode : BasicNode
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

    public override void DayAction()
    {

    }

    public override void HarvestAction()
    {
        SystemController.Instance.SetSystemPause(false);

        if (TimeBase.Instance.nowTimeState == TimeState.Day)
            SceneController.Instance.LoadSubScene(SceneType.Restaurant);
        else
            TimeBase.Instance.GoToSleep();
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
