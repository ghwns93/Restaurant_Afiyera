using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcSelectButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image highlightImage; // 선택되었을 때 보여줄 이미지
    public BasicNpcScript TargetNPC { get; private set; }

    public void Setup(BasicNpcScript npc)
    {
        TargetNPC = npc;
        nameText.text = npc.npcName;
        SetHighlight(false);
    }

    public void SetHighlight(bool isSelected)
    {
        // 선택한 이미지 활성화/비활성화
        Color color = highlightImage.color;
        color.a = isSelected ? 1f : 0f;
        highlightImage.color = color;
    }

    // 마우스로 직접 클릭했을 때 (Unity Button Event에 연결)
    public void ClickInteraction()
    {
        if (TargetNPC != null)
        {
            TargetNPC.NpcInteraction();
        }
    }
}