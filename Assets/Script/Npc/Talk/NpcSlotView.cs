using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 대화씬 화면에 NPC 초상화를 표시하는 슬롯 하나.
// 슬롯 프리팹(Image + CanvasGroup)에 붙여서 사용.
[RequireComponent(typeof(CanvasGroup))]
public class NpcSlotView : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float slideOffsetY = 30f; // 등장 시 아래→위로 살짝 떠오르는 연출

    [Header("말하는 사람 강조")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 비발화자는 어둡게

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 basePos;
    private Coroutine currentAnim;

    public string CurrentNpcId { get; private set; }
    public bool IsOccupied => !string.IsNullOrEmpty(CurrentNpcId);

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
        canvasGroup.alpha = 0f;
    }

    // NPC 등장
    public void Show(string npcId, Sprite portrait)
    {
        CurrentNpcId = npcId;
        portraitImage.sprite = portrait;
        portraitImage.enabled = portrait != null;

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(FadeRoutine(true));
    }

    // NPC 퇴장
    public void Hide()
    {
        CurrentNpcId = null;
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(FadeRoutine(false));
    }

    // 현재 말하는 NPC인지에 따라 밝기 조절
    public void SetSpeaking(bool speaking)
    {
        if (portraitImage != null)
            portraitImage.color = speaking ? activeColor : inactiveColor;
    }

    private IEnumerator FadeRoutine(bool fadeIn)
    {
        float from = canvasGroup.alpha;
        float to = fadeIn ? 1f : 0f;
        Vector2 fromPos = fadeIn ? basePos - new Vector2(0, slideOffsetY) : basePos;
        Vector2 toPos = basePos;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeDuration);
            // easeOutQuad
            p = 1f - (1f - p) * (1f - p);
            canvasGroup.alpha = Mathf.Lerp(from, to, p);
            if (fadeIn) rect.anchoredPosition = Vector2.Lerp(fromPos, toPos, p);
            yield return null;
        }
        canvasGroup.alpha = to;
        rect.anchoredPosition = toPos;
        currentAnim = null;
    }
}
