using UnityEngine;
using UnityEngine.UI;

public class BuildListBtnMaker : MonoBehaviour
{
    [SerializeField] private GameObject BuildBtnPrefab;

    [SerializeField] private GameObject BuildListPanel;

    [SerializeField] private BuildController bc;

    private void Start()
    {
        SetBuildButton();
    }

    public void SetBuildButton()
    {
        ClearBuildListPanel();

        var buildItems = BuildDicManager.Instance.GetAllDataList();

        foreach(var item in buildItems)
        {
            GameObject buildBtn = Instantiate(BuildBtnPrefab, BuildListPanel.transform);

            var shopUnlockItem = item.GetComponent<ShopUnlockableItem>();
            var nodeInfo = item.GetComponent<BasicNode>();
            Button btn = buildBtn.GetComponent<Button>();

            if (nodeInfo.IsBuildable)
            {
                if (shopUnlockItem != null)
                {
                    //if (shopUnlockItem.IsUnlockedByDefault)
                    {
                        int count = BuildableCountManager.Instance.GetBuildableCount(nodeInfo.NodeId);

                        if (count <= 0)
                        {
                            btn.interactable = false;
                        }
                    }
                }

                btn.onClick.AddListener(() => bc.SelectBuilding(item));

                var btnSetting = buildBtn.GetComponent<SetShopButtonInfo>();

                btnSetting.SetButton(nodeInfo.NodeName, "", null);
            }
            else
            {
                Debug.LogError("설치 할 수 없는 건물입니다.");
                Destroy(buildBtn);
            }
        }
    }

    public void ClearBuildListPanel()
    {
        if (BuildListPanel == null) return;

        for (int i = BuildListPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(BuildListPanel.transform.GetChild(i).gameObject);
        }
    }
}
