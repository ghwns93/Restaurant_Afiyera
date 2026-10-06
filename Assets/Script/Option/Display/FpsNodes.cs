using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FpsNodes : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private GameObject arrowImages;

    private int nodeIndex;
    private DisplaySettings displaySettings;

    public void Initialize(int index, int fpsValue, DisplaySettings settings)
    {
        nodeIndex = index;
        displaySettings = settings;

        if (fpsText != null)
        {
            fpsText.text = $"{fpsValue} FPS";
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
            displaySettings.OnFpsNodeHover(nodeIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (displaySettings != null)
        {
            displaySettings.OnFpsNodeExit();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (displaySettings != null)
        {
            displaySettings.SetFPSLimit(nodeIndex);
            displaySettings.UpdateFpsSelectedIndex(nodeIndex);
        }
    }
}