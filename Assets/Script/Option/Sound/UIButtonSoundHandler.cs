using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    // 마우스를 올렸을 때 (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        SFXPlayer.Play(SoundName.SFX_CursorHover); // 기존 SoundName 매칭
    }

    // 마우스를 눌렀을 때 (Click Down)
    public void OnPointerDown(PointerEventData eventData)
    {
        SFXPlayer.Play(SoundName.SFX_ClickDown); // 기존 SoundName 매칭
    }

    // 마우스를 뗐을 때 (Click Up)
    public void OnPointerUp(PointerEventData eventData)
    {
        SFXPlayer.Play(SoundName.SFX_ClickUp); // 기존 SoundName 매칭
    }
}
