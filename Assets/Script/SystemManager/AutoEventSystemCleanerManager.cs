using UnityEngine;
using UnityEngine.EventSystems;

public class AutoEventSystemCleanerManager : MonoBehaviour
{
    void Awake()
    {
        // 씬 내에 이미 EventSystem이 있는지 확인
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            // 내가 메인이 아니라면 (이미 하나가 있다면) 자신을 파괴
            Destroy(gameObject);
        }
    }
}
