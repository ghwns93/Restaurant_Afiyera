using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NpcTalkButton : MonoBehaviour
{
    private NpcInteractionBase targetNib;

    private GameObject actor;

    public Image highlightImage; // 선택되었을 때 보여줄 이미지
    public TextMeshProUGUI nameText;

    public Button button;

    public void Setup(NpcInteractionBase nib, GameObject actor)
    {
        targetNib = nib;

        this.actor = actor;

        nameText.text = nib.dialogueKey;

        button.onClick.AddListener(Onclick);

        SetHighlight(false);
    }

    public void Onclick()
    {
        targetNib.Execute(actor);
    }

    public void SetHighlight(bool isSelected)
    {
        // 선택한 이미지 활성화/비활성화
        Color color = highlightImage.color;
        color.a = isSelected ? 1f : 0f;
        highlightImage.color = color;
    }
}
