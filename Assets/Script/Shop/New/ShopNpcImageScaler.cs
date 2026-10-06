using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ShopNpcImageScaler : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private MaskableGraphic targetGraphic;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetNPCImage(Sprite newSprite)
    {
        if (newSprite == null || targetGraphic == null) return;

        if (targetGraphic is Image imageComp)
        {
            imageComp.sprite = newSprite;
        }
        else if (targetGraphic is RawImage rawImageComp)
        {
            rawImageComp.texture = newSprite.texture;
        }

        float originalWidth = newSprite.rect.width;
        float originalHeight = newSprite.rect.height;

        if (originalWidth <= 0 || originalHeight <= 0) return;

        float imageAspectRatio = originalHeight / originalWidth;
        float currentHeight = rectTransform.rect.height;

        float newWidth = currentHeight / imageAspectRatio;

        // 5. RectTransform의 sizeDelta를 변경하여 세로 크기 적용
        // sizeDelta는 (Width, Height)를 나타냅니다.
        Vector2 size = rectTransform.sizeDelta;
        size.x = newWidth; // 계산된 새로운 높이 적용
        rectTransform.sizeDelta = size;

        // (선택사항) 이미지 찌그러짐 방지
        targetGraphic.GetComponent<RectTransform>().sizeDelta = size; 
    }
}
