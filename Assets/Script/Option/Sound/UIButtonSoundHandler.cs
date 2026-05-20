using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sound Settings")]
    public SoundName hoverSound = SoundName.SFX_CursorHover;
    public SoundName downSound = SoundName.SFX_ClickDown;
    public SoundName upSound = SoundName.SFX_ClickUp;

    // 마우스를 올렸을 때 (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySfxSound(hoverSound);
    }

    // 마우스를 눌렀을 때 (Click Down)
    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySfxSound(downSound);
    }

    // 마우스를 뗐을 때 (Click Up)
    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySfxSound(upSound);
    }

    void Reset()
    {
        // 컴포넌트가 처음 붙을 때 기본값을 자동으로 할당함
        hoverSound = SoundName.SFX_CursorHover;
        downSound = SoundName.SFX_ClickDown;
        upSound = SoundName.SFX_ClickUp;
    }
}
