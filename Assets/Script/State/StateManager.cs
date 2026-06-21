using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private Dictionary<WorkerStateType, float> workerStates = new Dictionary<WorkerStateType, float>();

    public static StateManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetDefaultState();
        }
        else
        {
            Destroy(this);
        }
    }

    private void SetDefaultState()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(WorkerStateType)).Length; i++)
        {
            workerStates[(WorkerStateType)i] = 0f;
        }
    }

    public void SetState(WorkerStateType stateType, float value)
    {
        if (workerStates.ContainsKey(stateType))
        {
            workerStates[stateType] += value;
        }
        else
        {
            workerStates[stateType] = value;
        }
    }
}

public enum WorkerStateType
{
    HarvestValue,
    CookSpeed,
    MoveSpeed
}