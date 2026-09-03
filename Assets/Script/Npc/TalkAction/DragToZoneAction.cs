using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// [대화 액션] startPos에 이미지 프리팹 생성 → 유저가 드래그해서 dropZone 영역 안에 놓으면 완료
// 영역 밖에 놓으면 시작 위치로 되돌아가고 다시 시도 가능.
// DialogueActionRegistry의 자식 오브젝트에 붙이고 actionId를 지정해서 사용.
public class DragToZoneAction : DialogueActionBase
{
    [Header("생성 설정")]
    [SerializeField] private Image imagePrefab;         // 드래그할 이미지 프리팹 (Raycast Target 필수)
    [SerializeField] private RectTransform spawnParent; // 이미지가 생성될 부모 (보통 Canvas 아래)

    [Header("위치")]
    [SerializeField] private RectTransform startPos;    // 생성 지점 마커
    [SerializeField] private RectTransform dropZone;    // 드롭 판정 영역 (미리 배치한 Image의 RectTransform)

    [Header("동작")]
    [SerializeField] private bool snapToZoneCenter = true;  // 성공 시 영역 중앙에 스냅
    [SerializeField] private bool destroyOnDrop = false;    // 성공 시 이미지 제거 여부
    [SerializeField] private float returnDuration = 0.2f;   // 실패 시 되돌아가는 시간

    private Action onComplete;
    private Image spawnedImage;

    public override void Execute(Action onComplete)
    {
        this.onComplete = onComplete;

        spawnedImage = Instantiate(imagePrefab, spawnParent);
        RectTransform rect = spawnedImage.rectTransform;
        rect.position = startPos.position;

        dropZone.gameObject.SetActive(true); // 드롭 영역 표시

        var drag = spawnedImage.gameObject.AddComponent<DragRelay>();
        drag.owner = this;
    }

    // ===== DragRelay가 호출 =====

    private void OnDragMove(PointerEventData eventData)
    {
        RectTransform rect = spawnedImage.rectTransform;
        // 스크린 좌표 → 부모 기준 월드 좌표 변환 (Overlay/Camera 캔버스 모두 대응)
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                spawnParent, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
        {
            rect.position = worldPoint;
        }
    }

    private void OnDragEnd(PointerEventData eventData)
    {
        // 드롭 판정: 포인터가 dropZone 사각형 안에 있는가
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(
            dropZone, eventData.position, eventData.pressEventCamera);

        if (inside)
            Succeed();
        else
            StartCoroutine(ReturnToStart());
    }

    private void Succeed()
    {
        // 더 이상 드래그 불가
        var relay = spawnedImage.GetComponent<DragRelay>();
        if (relay != null) Destroy(relay);
        spawnedImage.raycastTarget = false;

        if (snapToZoneCenter)
            spawnedImage.rectTransform.position = dropZone.position;

        if (destroyOnDrop)
        {
            Destroy(spawnedImage.gameObject);
            spawnedImage = null;
        }

        dropZone.gameObject.SetActive(false); // 드롭 영역 표시

        var cb = onComplete;
        onComplete = null;
        cb?.Invoke(); // 대화 재개
    }

    private IEnumerator ReturnToStart()
    {
        RectTransform rect = spawnedImage.rectTransform;
        rect.GetComponent<DragRelay>().enabled = false; // 복귀 중 드래그 잠금

        Vector3 from = rect.position;
        Vector3 to = startPos.position;
        float t = 0f;
        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / returnDuration);
            p = 1f - (1f - p) * (1f - p); // easeOutQuad
            rect.position = Vector3.Lerp(from, to, p);
            yield return null;
        }
        rect.position = to;
        rect.GetComponent<DragRelay>().enabled = true; // 재시도 허용
    }

    // 생성된 이미지에 런타임으로 붙이는 드래그 감지용 소형 컴포넌트
    private class DragRelay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public DragToZoneAction owner;

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            if (enabled) owner.OnDragMove(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (enabled) owner.OnDragEnd(eventData);
        }
    }
}
