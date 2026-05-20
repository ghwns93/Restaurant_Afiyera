using System;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    // 전역적으로 접근 가능한 이벤트 (Action)
    public static event Action<bool> OnSystemStateChanged;
    private bool isSystemPaused = true;

    public static SystemController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetSystemPause(bool pause)
    {
        isSystemPaused = pause;
        // 등록된 모든 리스너에게 신호를 보냄 (null이 아닐 때만 실행)
        OnSystemStateChanged?.Invoke(isSystemPaused);
    }
}
