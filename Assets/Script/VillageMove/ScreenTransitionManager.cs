using System.Collections;
using UnityEngine;

public enum Direction
{
    Right,  // 오른쪽으로 진행 (검은 화면이 오른쪽 -> 왼쪽으로 들어옴)
    Left,   // 왼쪽으로 진행 (검은 화면이 왼쪽 -> 오른쪽으로 들어옴)
    Up,     // 위쪽으로 진행 (검은 화면이 아래 -> 위로 들어옴)
    Down    // 아래쪽으로 진행 (검은 화면이 위 -> 아래로 들어옴)
}

public class ScreenTransitionManager : MonoBehaviour
{
    public static ScreenTransitionManager Instance;

    [Header("UI Elements")]
    [SerializeField] private RectTransform blackPanel;
    [SerializeField] private float transitionDuration = 0.5f;

    private Vector2 screenSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        screenSize = new Vector2(Screen.width, Screen.height);
        blackPanel.gameObject.SetActive(false);
    }

    public IEnumerator PlayTransition(Direction moveDir, System.Action onOpaqueCallback)
    {
        blackPanel.gameObject.SetActive(true);

        // 1. 방향에 따른 시작, 중간, 끝 위치 설정
        Vector2 startPos = Vector2.zero;
        Vector2 centerPos = Vector2.zero;
        Vector2 endPos = Vector2.zero;

        switch (moveDir)
        {
            case Direction.Right: // 캐릭터가 우측 이동 -> 연출은 오른쪽에서 들어와서 왼쪽으로 퇴장
                startPos = new Vector2(screenSize.x, 0);
                centerPos = Vector2.zero;
                endPos = new Vector2(-screenSize.x, 0);
                break;
            case Direction.Left: // 캐릭터가 좌측 이동 -> 연출은 왼쪽에서 들어와서 오른쪽으로 퇴장
                startPos = new Vector2(-screenSize.x, 0);
                centerPos = Vector2.zero;
                endPos = new Vector2(screenSize.x, 0);
                break;
            case Direction.Up: // 캐릭터가 위로 이동 -> 연출은 아래에서 들어와서 위로 퇴장
                startPos = new Vector2(0, -screenSize.y);
                centerPos = Vector2.zero;
                endPos = new Vector2(0, screenSize.y);
                break;
            case Direction.Down: // 캐릭터가 아래로 이동 -> 연출은 위에서 들어와서 아래로 퇴장
                startPos = new Vector2(0, screenSize.y);
                centerPos = Vector2.zero;
                endPos = new Vector2(0, -screenSize.y);
                break;
        }

        // 2. 화면 채우기 (Start -> Center)
        blackPanel.anchoredPosition = startPos;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            blackPanel.anchoredPosition = Vector2.Lerp(startPos, centerPos, t);
            yield return null;
        }
        blackPanel.anchoredPosition = centerPos;

        // 3. 화면이 완전히 가려졌을 때 캐릭터 위치 이동 & 카메라 영역 변경 실행
        onOpaqueCallback?.Invoke();
        yield return new WaitForSeconds(0.1f); // 약간의 대기시간

        // 4. 화면 밝아지기 (Center -> End)
        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            blackPanel.anchoredPosition = Vector2.Lerp(centerPos, endPos, t);
            yield return null;
        }
        blackPanel.anchoredPosition = endPos;

        blackPanel.gameObject.SetActive(false);
    }
}