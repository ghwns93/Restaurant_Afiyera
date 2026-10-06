using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingPanelContoller : MonoBehaviour
{
    // 버튼과 판넬을 하나의 쌍으로 묶어주는 구조체
    [System.Serializable]
    public struct TabPair
    {
        public string tabName; // 인스펙터 식별용 (없어도 무방)
        public Button button;
        public GameObject panel;
        public GameObject selectedImage;
    }

    [Header("탭 설정")]
    public List<TabPair> tabs;

    private void OnEnable()
    {
        UIOpenRegistry.RegisterUI();
    }

    private void OnDisable()
    {
        UIOpenRegistry.UnregisterUI();
    }

    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            // 각 버튼에 클릭 이벤트 할당
            if (tabs[i].button != null)
            {
                tabs[i].button.onClick.AddListener(() => OnTabButtonClicked(index));

                tabs[i].panel.SetActive(false); // 초기에는 모든 패널 끄기
            }
        }

        // 초기 상태: 첫 번째 탭만 켜기
        if (tabs.Count > 0) ShowPanel(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeSettingPanel();
        }
    }

    private void OnTabButtonClicked(int index)
    {
        ShowPanel(index);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].panel != null)
            {
                // 인덱스가 맞으면 true, 아니면 false
                tabs[i].panel.SetActive(i == index);
                tabs[i].selectedImage.SetActive(i == index); // 선택된 탭 표시 이미지 활성화/비활성화
            }
        }
    }

    public void closeSettingPanel()
    {
        SceneController.Instance.OptionSceneOpenOrClose();
    }
}
