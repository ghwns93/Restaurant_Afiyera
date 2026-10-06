using TMPro;
using UnityEngine;

public class InventorySceneManager : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private InventorySceneScript inventoryRoot;
    [SerializeField] private Canvas parentCanvas;           // 부모 캔버스 (Screen Space - Camera 또는 Overlay)

    [Header("Description Settings")]
    [SerializeField] private GameObject descriptPanel;       // 노란 박스 패널
    [SerializeField] private TextMeshProUGUI descriptText;   // 내부 텍스트
    [SerializeField] private Vector2 mouseOffset = new Vector2(15f, -15f); // 마우스 간격
    private RectTransform panelRectTransform;

    private static InventorySceneManager instance;
    public static InventorySceneManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DescriptSetting();
        CloseUI();
    }

    private void LateUpdate()
    {
        // 툴팁이 켜져 있는 동안 마우스를 부드럽게 실시간 추적
        if (descriptPanel != null && descriptPanel.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void DescriptSetting()
    {
        if (descriptPanel != null)
        {
            panelRectTransform = descriptPanel.GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
            descriptPanel.SetActive(false); // 시작할 때 끄기
        }
    }

    public void ShowTooltip(string message)
    {
        if (descriptPanel != null)
        {
            if (descriptText != null) descriptText.text = message;

            // ★ 중요: 툴팁을 켤 때 Hierarchy 최하단(가장 앞으로)으로 이동시켜 다른 UI 뒤로 숨지 않게 함
            descriptPanel.transform.SetAsLastSibling();

            // 툴팁이 켜지는 순간의 마우스 위치에 딱 한 번만 위치를 고정
            UpdatePosition();

            descriptPanel.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (descriptPanel != null)
        {
            descriptPanel.SetActive(false);
        }
    }

    private void UpdatePosition()
    {
        if (panelRectTransform == null) return;

        // 마우스 스크린 좌표에 오프셋을 더해 툴팁 위치 지정
        Vector2 mousePosition = Input.mousePosition;
        panelRectTransform.position = mousePosition + mouseOffset;
    }

    public void OpenUI()
    {
        SystemController.Instance.SetSystemPause(false);
        inventoryRoot.InitSlots();
        uiRoot.SetActive(true);
    }

    public void CloseUI()
    {
        SystemController.Instance.SetSystemPause(true);
        uiRoot.SetActive(false);
    }
}
