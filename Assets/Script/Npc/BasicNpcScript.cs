using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicNpcScript : MonoBehaviour
{
    public string npcName = "상인";

    public NpcInteractionBase npcInteractionBase; // NPC가 가진 상호작용 정보 (예: 대화, 퀘스트 등)
    public List<NpcInteractionBase> npcInteractionList;

    private void Start()
    {
        InputInteraction();
    }

    public void InputInteraction()
    {
        foreach (var interaction in npcInteractionList)
        {
            NpcInteractionManager.Instance.InputQuest(interaction, interaction.isUnRocked);
        }
    }

    public void NpcInteraction()
    {
        //NPC 상호작용 코드
        npcInteractionBase.Execute(gameObject);

        if (npcInteractionBase is NpcInteractionTalk)
        {
            var unlockedTalk = npcInteractionList
                               .Where(interaction => NpcInteractionManager.Instance.IsQuestCompleted(interaction))
                               .ToList();

            NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, gameObject);
        }
    }

    private void OnDetected() => NpcSelectEvents.OnNPCDetected?.Invoke(this);
    private void OnLost() => NpcSelectEvents.OnNPCLost?.Invoke(this);
}