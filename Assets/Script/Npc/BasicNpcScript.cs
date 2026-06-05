using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicNpcScript : MonoBehaviour
{
    public string npcName = "상인";

    public NpcInteractionBase npcInteractionBase; // NPC가 가진 상호작용 정보 (예: 대화, 퀘스트 등)
    [SerializeField]
    private List<NpcInteractionBase> npcInteractionList;

    private List<NpcInteractionBase> copyedNpcInteractionList = new List<NpcInteractionBase>();

    private void Start()
    {
        InputInteraction();
    }

    public void InputInteraction()
    {
        foreach (var interaction in npcInteractionList)
        {
            if (interaction != null)
            {
                var newInteraction = Instantiate(interaction);
                copyedNpcInteractionList.Add(newInteraction);
            }
        }

        foreach (var interaction in copyedNpcInteractionList)
        {
            NpcInteractionManager.Instance.InputQuest(interaction, interaction.isUnRocked);
        }
    }

    private void SetNpcInteractionButton()
    {
        if (npcInteractionBase is NpcInteractionTalk)
        {
            var unlockedTalk = copyedNpcInteractionList
                               .Where(interaction => NpcInteractionManager.Instance.IsQuestCompleted(interaction))
                               .ToList();

            NpcTalkUIManager.Instance.ShowSelectionButtons(unlockedTalk, gameObject);
        }
    }

    public void NpcInteraction()
    {
        //NPC 상호작용 코드
        npcInteractionBase.Execute(gameObject);

        SetNpcInteractionButton();
    }

    private void OnDetected() => NpcSelectEvents.OnNPCDetected?.Invoke(this);
    private void OnLost() => NpcSelectEvents.OnNPCLost?.Invoke(this);
}