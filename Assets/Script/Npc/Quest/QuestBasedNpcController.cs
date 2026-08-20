using System.Collections.Generic;
using UnityEngine;

public class QuestBasedNpcController : MonoBehaviour
{
    private BasicNpcScript bns;
    private string npcId;

    private void OnEnable()
    {
        NpcInteractionManager.OnQuestStateChanged += HandleQuestStateChanged;
    }

    private void OnDisable()
    {
        NpcInteractionManager.OnQuestStateChanged -= HandleQuestStateChanged;
    }

    public void SetBns(BasicNpcScript bns, string npcId)
    {
        //BasicNpcScript 셋팅 완료 후 로드
        this.bns = bns;
        this.npcId = npcId;

        EvaluateState();
    }

    private void HandleQuestStateChanged(string targetNpcId, NpcInteractionBase quest)
    {
        if (bns != null)
        {
            if (npcId == targetNpcId)
            {
                EvaluateState();
            }
        }
    }

    public void EvaluateState()
    {
        if (bns != null)
        {
            var totInteraction = bns.CopyedNpcInteractionList;

            foreach (var item in totInteraction)
            {
                if (item is IQuestEventListener listener)
                {
                    if (item.CanActivate())
                    {
                        listener.OnEvaluateState(this.gameObject);
                        break;
                    }
                }
            }
        }
    }
}