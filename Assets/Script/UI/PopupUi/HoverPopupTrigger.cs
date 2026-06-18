using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HoverPopupTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject popupWindow; // 띄울 팝업창 오브젝트

    [Range(0f, 3f)]
    [SerializeField] private float hoverDelay = 0.5f; // 판정 시간 (0.5초)

    private Coroutine hoverCoroutine;
    protected GameObject popupPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(CheckHoverTimer());
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }

        if (popupPanel != null)
        {
            Destroy(popupPanel);

            popupPanel = null;
        }
    }

    // 일정시간을 버텨내면 팝업을 띄우는 타이머 코루틴
    private IEnumerator CheckHoverTimer()
    {
        // 설정한 시간 동안 대기
        yield return new WaitForSeconds(hoverDelay);

        // 시간이 다 되면 팝업창 활성화!
        if (popupWindow != null && popupPanel == null)
        {
            popupPanel = Instantiate(popupWindow, transform);

            SetItemInfo();
        }

        hoverCoroutine = null;
    }

    protected abstract void SetItemInfo();
}
