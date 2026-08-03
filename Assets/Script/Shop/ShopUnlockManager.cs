using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopUnlockManager : MonoBehaviour
{
    public static ShopUnlockManager Instance { get; private set; }

    // 해금 상태가 변경될 때 UI 등에 알려주기 위한 이벤트
    public static event Action<string> OnItemUnlocked;

    private HashSet<string> unlockedIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlockData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 해금 여부 확인
    public bool IsUnlocked(string id)
    {
        if (string.IsNullOrEmpty(id)) return true; // ID가 없으면 기본 해금으로 처리
        return unlockedIDs.Contains(id);
    }

    // 아이템 해금 처리
    public bool Unlock(string id)
    {
        if (IsUnlocked(id)) return false; // 이미 해금됨

        unlockedIDs.Add(id);
        SaveUnlockData(id);

        // 해금 이벤트 발생 (구독 중인 UI들이 즉시 갱신됨)
        OnItemUnlocked?.Invoke(id);
        return true;
    }

    // 데이터 저장 (단순 예시로 PlayerPrefs 사용, 이후 JSON이나 SaveSystem으로 교체 가능)
    private void SaveUnlockData(string id)
    {
        PlayerPrefs.SetInt("Unlock_" + id, 1);
        PlayerPrefs.Save();
    }

    // 데이터 로드
    private void LoadUnlockData()
    {
        // 씬 내에 있는 기본 해금 항목이나 이전에 저장된 항목을 로드하는 로직
        // 테스트용: PlayerPrefs 키를 직접 감지하거나 해금 시 저장된 목록을 따로 관리할 수 있습니다.
    }
}
