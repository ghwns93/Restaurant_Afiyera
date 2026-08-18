using UnityEngine;
using UnityEngine.Events;

// 월드 스페이스 Sprite전용 버튼 스크립트
public class WorldButton : MonoBehaviour
{
    public UnityEvent onClick = new UnityEvent();

    private void OnMouseDown()
    {
        // 2D Collider 클릭 시 이벤트 실행
        onClick?.Invoke();
    }
}