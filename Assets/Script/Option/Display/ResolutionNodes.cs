using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ResolutionNodes : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private GameObject arrowImages;

    private int nodeIndex;
    private DisplaySettings displaySettings;

    public void Initialize(int index, ResolutionOption option, DisplaySettings settings)
    {
        nodeIndex = index;
        displaySettings = settings;

        if (resolutionText != null)
        {
            resolutionText.text = $"{option.width} x {option.height}";
        }

        SetArrowsActive(false);
    }

    public void SetArrowsActive(bool isActive)
    {
        if (arrowImages != null) arrowImages.SetActive(isActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (displaySettings != null)
        {
            displaySettings.OnNodeHover(nodeIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (displaySettings != null)
        {
            displaySettings.OnNodeExit();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (displaySettings != null)
        {
            displaySettings.SetResolution(nodeIndex);
            // 클릭 즉시 해당 노드가 선택된 상태로 갱신되도록 처리 요청 가능
            displaySettings.UpdateSelectedIndex(nodeIndex);
        }
    }
}