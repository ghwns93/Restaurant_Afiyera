using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField]
    private List<BuffEffect> activeBuff = new List<BuffEffect>();

    internal static BuffManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddBuff(BuffEffect buff)
    {
        activeBuff.Add(Instantiate(buff));
        buff.Apply(gameObject);
    }

    public void DayCheck(int day)
    {
        activeBuff.ForEach(buff => buff.duration -= day);
        List<BuffEffect> expiredBuffs = activeBuff.FindAll(buff => buff.duration == 0);

        foreach (BuffEffect buff in expiredBuffs)
        {
            RemoveBuff(buff);
        }
    }

    public void RemoveBuff(BuffEffect buff)
    {
        if (activeBuff.Contains(buff))
        {
            activeBuff.Remove(buff);
            buff.Remove(gameObject);
        }
    }
}
