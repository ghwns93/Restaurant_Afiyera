using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// [대화 액션] startPos에 이미지를 생성 → 유저가 클릭하면 endPos로 n초 동안 이동 → 도착 시 완료
// DialogueActionRegistry의 자식 오브젝트에 붙이고 actionId를 지정해서 사용.
public class ClickMoveImageAction : DialogueActionBase
{
    [Header("생성 설정")]
    [SerializeField] private Image imagePrefab;       // 생성할 이미지 프리팹 (Raycast Target 켜져 있어야 클릭됨)
    [SerializeField] private RectTransform spawnParent; // 이미지가 생성될 부모 (보통 Canvas 아래 오브젝트)

    [Header("위치")]
    [SerializeField] private RectTransform startPos;  // 시작 지점 마커
    [SerializeField] private RectTransform endPos;    // 도착 지점 마커

    [Header("이동")]
    [SerializeField] private float moveDuration = 1.0f; // n초
    [SerializeField] private bool destroyOnArrive = true; // 도착 후 이미지 제거 여부

    private Action onComplete;
    private Image spawnedImage;

    public override void Execute(Action onComplete)
    {
        this.onComplete = onComplete;

        // startPos 위치에 이미지 생성
        spawnedImage = Instantiate(imagePrefab, spawnParent);
        RectTransform rect = spawnedImage.rectTransform;
        rect.position = startPos.position; // 마커의 월드 위치를 그대로 사용 (부모가 달라도 안전)

        // 클릭 감지 컴포넌트를 런타임에 부착
        var clickable = spawnedImage.gameObject.AddComponent<ClickRelay>();
        clickable.onClick = OnImageClicked;
    }

    private void OnImageClicked()
    {
        // 중복 클릭 방지: 이동 시작 후엔 클릭 무시
        var relay = spawnedImage.GetComponent<ClickRelay>();
        if (relay != null) Destroy(relay);
        spawnedImage.raycastTarget = false;

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        RectTransform rect = spawnedImage.rectTransform;
        Vector3 from = rect.position;
        Vector3 to = endPos.position;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / moveDuration);
            // easeInOutQuad: 출발/도착이 부드러움
            p = p < 0.5f ? 2f * p * p : 1f - Mathf.Pow(-2f * p + 2f, 2f) / 2f;
            rect.position = Vector3.Lerp(from, to, p);
            yield return null;
        }
        rect.position = to;

        if (destroyOnArrive && spawnedImage != null)
            Destroy(spawnedImage.gameObject);
        spawnedImage = null;

        // 완료 통지 → 대화 재개
        var cb = onComplete;
        onComplete = null;
        cb?.Invoke();
    }

    // 생성된 이미지에 런타임으로 붙이는 클릭 감지용 소형 컴포넌트
    private class ClickRelay : MonoBehaviour, IPointerClickHandler
    {
        public Action onClick;
        public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke();
    }
}