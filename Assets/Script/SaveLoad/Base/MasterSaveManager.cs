using System;
using System.Linq;
using UnityEngine;

public class MasterSaveManager : MonoBehaviour
{
    public static MasterSaveManager Instance { get; private set; }

    // 임시 매니저들이 구독할 로드 완료 이벤트
    public static event Action<SaveData> OnSaveDataLoaded;

    // 현재 게임의 모든 최신 데이터를 실시간으로 들고 있는 마스터 보따리(임시)
    // 나중에 Json 이든 바이너리든 저장방식 변경 필요
    private SaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // [핵심] 유니티가 씬 로드를 끝내면 자동으로 호출됨
    public void OnSceneLoadedComplete()
    {
        // 게임을 처음 켰다면 마스터 데이터 보따리를 새로 생성 (메모리 초기화)
        if (currentSaveData == null)
        {
            currentSaveData = new SaveData();
        }

        // 씬 내의 모든 임시 매니저들에게 데이터 뿌리기
        OnSaveDataLoaded?.Invoke(currentSaveData);
    }

    // [세이브 타이밍에 호출] 씬 전환 직전이나 저장 버튼 누를 때 명시적으로 호출하세요!
    public void CollectAllData()
    {
        if (currentSaveData == null) return;

        // 현재 씬에 존재하는 모든 ISaveable(임시 매니저들)을 수집
        var saveableManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();

        // 각 매니저들에게 "너희가 들고 있던 최신 데이터 여기에 다 담아!"라고 지시
        foreach (var manager in saveableManagers)
        {
            manager.BindSaveData(currentSaveData);
        }
    }
}