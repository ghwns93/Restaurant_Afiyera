using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicNpcScript : MonoBehaviour
{
    public int npcCode;

    public string npcName = "상인";

    public TargetType targetType;

    public NpcInteractionBase npcInteractionBase; // NPC가 가진 상호작용 정보 (예: 대화, 퀘스트 등)
    [SerializeField]
    private List<NpcInteractionBase> npcInteractionList;

    private List<NpcInteractionBase> copyedNpcInteractionList = new List<NpcInteractionBase>();

    private string myNpcId;

    public List<NpcInteractionBase> CopyedNpcInteractionList { get => copyedNpcInteractionList; set => copyedNpcInteractionList = value; }
    public string MyNpcId { get => myNpcId; }

    private void Start()
    {
        CreateThisId();
        InputInteraction();

        var questSet = gameObject.GetComponent<QuestBasedNpcController>();

        if(questSet != null)
        {
            questSet.SetBns(this, myNpcId);
        }
    }

    public void CreateThisId()
    {
        //myNpcId = System.Guid.NewGuid().ToString();

        //Debug.Log("새로운 건물 " + npcName + " 생성, ID: " + myNpcId);

        if(targetType == TargetType.Building)
        {
            myNpcId = string.Format("{0}.{1}.{2}", transform.position.x, transform.position.y, transform.position.z);
        }
        else if (targetType == TargetType.Npc)
        {
            myNpcId = string.Format("npc.{0}.{1}", npcCode, npcName);
        }
    }

    public void InputInteraction()
    {
        foreach (var interaction in npcInteractionList)
        {
            if (interaction != null)
            {
                var newInteraction = Instantiate(interaction);

                newInteraction.targetNpcId = MyNpcId;

                CopyedNpcInteractionList.Add(newInteraction);
            }
        }
    }

    public void SetNpcInteractionButton()
    {
        if (npcInteractionBase is NpcInteractionTalk)
        {
            List<NpcInteractionBase> unlockedTalk = new List<NpcInteractionBase>();

            foreach (var talk in CopyedNpcInteractionList)
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

    public void ResetNpcInteraction(QuestInteractionType qit)
    {
        foreach (var interaction in CopyedNpcInteractionList)
        {
            if (interaction.questInteractionType == qit)
            {
                NpcInteractionManager.Instance.ResetQuest(MyNpcId, interaction);
                break;
            }
        }
    }

    private void OnDetected() => NpcSelectEvents.OnNPCDetected?.Invoke(this);
    private void OnLost() => NpcSelectEvents.OnNPCLost?.Invoke(this);
}

public enum TargetType { Building, Npc }