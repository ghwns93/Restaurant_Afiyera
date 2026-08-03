using UnityEngine;
using UnityEngine.UI;

public class BuildListBtnMaker : MonoBehaviour
{
    [SerializeField] private GameObject BuildBtnPrefab;

    [SerializeField] private GameObject BuildListPanel;

    [SerializeField] private BuildController bc;

    void Start()
    {
        SetBuildButton();
    }

    private void SetBuildButton()
    {
        var buildItems = BuildDicManager.Instance.GetAllDataList();

        foreach(var item in buildItems)
        {
            GameObject buildBtn = Instantiate(BuildBtnPrefab, BuildListPanel.transform);

            var shopUnlockItem = item.GetComponent<ShopUnlockableItem>();
            var nodeInfo = item.GetComponent<BasicNode>();
            Button btn = buildBtn.GetComponent<Button>();

            if (shopUnlockItem != null && nodeInfo.IsBuildable)
            {
                if(!shopUnlockItem.IsUnlockedByDefault)
                {
                   int count = BuildableCountManager.Instance.GetBuildableCount(nodeInfo.NodeId);

                    if(count <= 0)
                    {
                        btn.interactable = false;
                    }
                }

                btn.onClick.AddListener(() => bc.SelectBuilding(item));

                var btnSetting = buildBtn.GetComponent<SetShopButtonInfo>();

                btnSetting.SetButton(nodeInfo.NodeName, "", null);
            }
            else
            {
                Debug.LogError("ShopUnlockableItem 이 없는 건물입니다.");
                Destroy(buildBtn);
            }
        }
    }
}
