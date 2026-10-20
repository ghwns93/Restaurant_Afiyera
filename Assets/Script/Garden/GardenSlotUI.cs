using UnityEngine;
using UnityEngine.UI;

public class GardenSlotUI : MonoBehaviour
{
    public int slotIndex;

    [Header("UI Buttons")]
    public Button objectSelectBtn;   // 1번 버튼 (오브젝트 선택 창 열기)
    public Button materialSelectBtn; // 2번 버튼 (재료 선택 창 열기)

    [Header("UI Icons")]
    public Image objectIconImage;   // 1번 선택 아이콘 이미지
    public Image materialIconImage; // 2번 선택 아이콘 이미지

    public void InitSlot(int index)
    {
        slotIndex = index;

        objectSelectBtn.onClick.AddListener(OnClickObjectButton);
        materialSelectBtn.onClick.AddListener(OnClickMaterialButton);

        // 초기 생성 시 빈 상태로 세팅 (버튼은 살아있고 아이콘만 투명하게 처리)
        UpdateUI(null, null);
    }

    private void OnClickObjectButton()
    {
        GardenUIManager.Instance.OpenObjectSelectionPanel(slotIndex, this);
    }

    private void OnClickMaterialButton()
    {
        GardenUIManager.Instance.OpenMaterialSelectionPanel(slotIndex, this);
    }

    public void UpdateUI(GardenObjectData gardenObj, ItemIngredientData material)
    {
        // 1번 오브젝트 아이콘 처리
        if (gardenObj != null && gardenObj.Icon != null)
        {
            objectIconImage.sprite = gardenObj.Icon;
            SetImageAlpha(objectIconImage, 1.0f); // 선명하게 보이도록 알파값 1
        }
        else
        {
            objectIconImage.sprite = null;
            SetImageAlpha(objectIconImage, 0.0f); // 이미지가 없으면 투명하게 처리 (버튼 클릭은 정상 작동)
        }

        // 2번 재료 아이콘 처리
        if (material != null && material.Icon != null)
        {
            materialIconImage.sprite = material.Icon;
            SetImageAlpha(materialIconImage, 1.0f);
        }
        else
        {
            materialIconImage.sprite = null;
            SetImageAlpha(materialIconImage, 0.0f);
        }
    }

    // 아이콘 이미지의 투명도(Alpha)를 조절하는 헬퍼 메서드
    private void SetImageAlpha(Image img, float alpha)
    {
        if (img != null)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
}