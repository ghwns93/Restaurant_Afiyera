using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class ShopNpcImageScaler_v2 : MonoBehaviour
{
    private AspectRatioFitter aspectRatioFitter;

    // 대상이 되는 이미지 컴포넌트 (Image 또는 RawImage)
    private MaskableGraphic targetGraphic;

    private void Awake()
    {
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
        targetGraphic = GetComponent<MaskableGraphic>();

        if (targetGraphic == null)
        {
            // 만약 Image 컴포넌트가 같은 오브젝트에 없다면 자식에서 찾거나 오류 로그
            Debug.LogError("NPCImageScaler_V2 스크립트가 부착된 오브젝트에 Image/RawImage 컴포넌트가 필요합니다.");
        }
    }

    /// <summary>
    /// 새로운 NPC 스프라이트(또는 텍스처)가 들어왔을 때 호출
    /// </summary>
    public void SetNPCImage(Sprite newSprite)
    {
        if (newSprite == null || targetGraphic == null || aspectRatioFitter == null) return;

        // 1. 이미지 컴포넌트에 새 스프라이트 할당 (RawImage라면 texture를 사용)
        if (targetGraphic is Image imageComp)
        {
            imageComp.sprite = newSprite;
        }
        else if (targetGraphic is RawImage rawImageComp)
        {
            rawImageComp.texture = newSprite.texture;
        }

        // 2. 원본 이미지의 가로 세로 비율 계산 (Width / Height)
        float originalWidth = newSprite.rect.width;
        float originalHeight = newSprite.rect.height;

        if (originalWidth <= 0 || originalHeight <= 0) return;

        float imageAspectRatio = originalWidth / originalHeight;

        // 3. AspectRatioFitter에 계산된 비율 적용
        // Aspect Ratio = Width / Height
        aspectRatioFitter.aspectRatio = imageAspectRatio;

        // 4. (선택사항) 부모 영역이 너무 작아 이미지가 뭉개질 경우 최소 크기 보장 로직 추가 가능
        // 하지만 일반적인 경우 Fit In Parent로 충분합니다.
    }
}
