using System.Collections;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private RectTransform[] menuButtons; // 설정, 저장, 떠하기 버튼 순서대로 등록

    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 150f;   // 아래에서 올라올 거리
    [SerializeField] private float animationDuration = 0.4f; // 올라오는 데 걸리는 시간
    [SerializeField] private float delayBetweenButtons = 0.1f; // 버튼 간의 파도 타기 딜레이

    private Vector2[] targetPositions;
    private bool isMenuOpen = false;

    private void Awake()
    {
        // 각 버튼의 원래 위치(목표 위치)를 미리 저장해둡니다.
        targetPositions = new Vector2[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            targetPositions[i] = menuButtons[i].anchoredPosition;

            // 시작할 때는 버튼들을 아래로 내리고 숨겨둡니다.
            menuButtons[i].anchoredPosition = targetPositions[i] - new Vector2(0, moveDistance);
            menuButtons[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(SystemController.Instance != null && SystemController.Instance.IsSystemPaused == false)
        {
            return;
        }

        // ESC 키를 누르면 메뉴 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    private void OpenMenu()
    {
        isMenuOpen = true;
        StopAllCoroutines();
        StartCoroutine(AnimateButtonsIn());
    }

    private void CloseMenu()
    {
        isMenuOpen = false;
        StopAllCoroutines();
        StartCoroutine(AnimateButtonsOut());
    }

    private IEnumerator AnimateButtonsIn()
    {
        // 1. 먼저 모든 버튼 오브젝트를 활성화
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].gameObject.SetActive(true);
        }

        // 2. 왼쪽부터 오른쪽 순서로 파도 타듯이 위로 슬라이드
        for (int i = 0; i < menuButtons.Length; i++)
        {
            StartCoroutine(SlideButton(menuButtons[i], menuButtons[i].anchoredPosition, targetPositions[i], animationDuration));
            yield return new WaitForSeconds(delayBetweenButtons); // 딜레이를 주어 파도 타기 효과 연출
        }
    }

    private IEnumerator AnimateButtonsOut()
    {
        // 닫을 때는 역순 또는 동시에 아래로 내리기
        for (int i = menuButtons.Length - 1; i >= 0; i--)
        {
            Vector2 hidePosition = targetPositions[i] - new Vector2(0, moveDistance);
            StartCoroutine(SlideButton(menuButtons[i], menuButtons[i].anchoredPosition, hidePosition, animationDuration));
            yield return new WaitForSeconds(delayBetweenButtons);
        }

        // 애니메이션 끝난 뒤 비활성화 처리를 위한 대기
        yield return new WaitForSeconds(animationDuration);

        if (!isMenuOpen)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                menuButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator SlideButton(RectTransform buttonRect, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 게임이 일시정지(Time.timeScale = 0) 되어도 동작하도록 unscaledDeltaTime 사용
            float t = elapsedTime / duration;

            // 부드럽게 감속하는 이징(Ease Out) 공식 적용
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            buttonRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        buttonRect.anchoredPosition = endPos;
    }

    public void OpenOption()
    {
        if(SceneController.Instance != null)
        {
            SceneController.Instance.OptionSceneOpenOrClose();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}