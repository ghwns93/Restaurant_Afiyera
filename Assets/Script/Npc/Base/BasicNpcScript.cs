using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicNpcScript : MonoBehaviour
{
    [SerializeField] private NpcBasicInfo npcBasicInfo;

    private List<NpcInteractionBase> copyedNpcInteractionList = new List<NpcInteractionBase>();

    public List<NpcInteractionBase> CopyedNpcInteractionList { get => copyedNpcInteractionList; set => copyedNpcInteractionList = value; }
    public string MyNpcId => npcBasicInfo.npcId;
    public NpcBasicInfo MyNpcBasicInfo => npcBasicInfo;

    private void Start()
    {
        StartRoutine();
    }

    public virtual void StartRoutine()
    {
        if(npcBasicInfo == null)
        {
            //Debug.LogError("NpcBasicInfo is not assigned in " + gameObject.name);
            return;
        }

        InputInteraction();
        SetNpcImage();

        var questSet = gameObject.GetComponent<QuestBasedNpcController>();

        if (questSet != null)
        {
            questSet.SetBns(this, MyNpcId);
        }
    }

    private void SetNpcImage()
    {
        if(npcBasicInfo.npcSdImage != null)
        {
            var npcSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (npcSpriteRenderer != null)
            {
                npcSpriteRenderer.sprite = npcBasicInfo.npcSdImage;
            }
        }
    }

    public void InputInteraction()
    {
        foreach (var interaction in npcBasicInfo.npcInteractionList)
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
        if (npcBasicInfo.npcInteractionBase is NpcInteractionTalk)
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
        npcBasicInfo.npcInteractionBase.Execute(gameObject);

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