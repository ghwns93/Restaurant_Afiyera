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

    private string myNpcId;

    private void Start()
    {
        CreateThisId();
        InputInteraction();
    }

    private void CreateThisId()
    {
        myNpcId = System.Guid.NewGuid().ToString();

        //Debug.Log("새로운 건물 " + npcName + " 생성, ID: " + myNpcId);
    }

    public void InputInteraction()
    {
        foreach (var interaction in npcInteractionList)
        {
            if (interaction != null)
            {
                var newInteraction = Instantiate(interaction);

                newInteraction.targetNpcId = myNpcId;

                copyedNpcInteractionList.Add(newInteraction);
            }
        }
    }

    private void SetNpcInteractionButton()
    {
        if (npcInteractionBase is NpcInteractionTalk)
        {
            List<NpcInteractionBase> unlockedTalk = new List<NpcInteractionBase>();

            foreach (var talk in copyedNpcInteractionList)
            {
                if (talk.CanActivate())
                {
                    unlockedTalk.Add(talk);
                }
            }

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