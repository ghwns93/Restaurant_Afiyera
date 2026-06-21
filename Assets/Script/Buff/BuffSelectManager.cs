using System.Collections.Generic;
using UnityEngine;

public class BuffSelectManager : MonoBehaviour
{
    [SerializeField] private int buffCount = 3; // 최대 버프 수

    private List<BuffEffect> tempSelectBuff = new List<BuffEffect>(); // 선택된 버프들

    public static BuffSelectManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public bool AddBuff(BuffEffect buff)
    {
        if(tempSelectBuff.Count >= buffCount)
        {
            Debug.Log("최대 버프 수를 초과했습니다.");
            return false;
        }

        tempSelectBuff.Add(buff);

        return true;
    }

    public void RemoveBuff(BuffEffect buff)
    {
        if(tempSelectBuff.Contains(buff)) tempSelectBuff.Remove(buff);
    }

    public void CommitBuff()
    {
        if(BuffManager.Instance != null)
        {
            foreach (BuffEffect buff in tempSelectBuff)
            {
                BuffManager.Instance.AddBuff(buff);
            }

            tempSelectBuff.Clear();

            SceneController.Instance.LoadSubScene(SceneType.Village);
        }
    }
}
