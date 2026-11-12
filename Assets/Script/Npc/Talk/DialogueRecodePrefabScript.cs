using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRecodePrefabScript : MonoBehaviour
{
    [SerializeField] private Image charaImage;
    [SerializeField] private TextMeshProUGUI charaNameText;
    [SerializeField] private TextMeshProUGUI contentsText;

    public void SetDialogueLogEntry(DialogueLogEntry entry)
    {
        var npc = NpcDicManager.Instance.GetData(entry.speakerName);

        if (npc != null && charaImage != null) charaImage.sprite = npc.npcIconImage;
        else if(charaImage != null) charaImage.gameObject.SetActive(false);

        if (charaNameText != null) charaNameText.text = npc == null? entry.speakerName:npc.npcName;
        if (contentsText != null) contentsText.text = entry.dialogueText;
    }
}
