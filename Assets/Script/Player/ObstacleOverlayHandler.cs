using UnityEngine;

public class ObstacleOverlayHandler : MonoBehaviour
{
    private SpriteRenderer overlayRenderer;
    private SpriteRenderer mySpriteRenderer; 
    private Transform overlayTrans;

    public float overlayAlpha = 0.5f; // 오버레이 투명도

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        // 자식 오버레이 오브젝트 자동 생성
        overlayTrans = transform.Find("PlayerOverlay");
        if (overlayTrans == null)
        {
            GameObject overlayObj = new GameObject("PlayerOverlay");
            overlayObj.transform.SetParent(transform);
            overlayObj.transform.localPosition = Vector3.zero;
            overlayObj.transform.localScale = Vector3.one;

            overlayRenderer = overlayObj.AddComponent<SpriteRenderer>(); 
            overlayTrans = overlayObj.transform;
        }
        else
        {
            overlayRenderer = overlayTrans.GetComponent<SpriteRenderer>();
        }

        // Sorting Order 자동 설정 (나무보다 2단계 앞서 그리기)
        if (mySpriteRenderer != null && overlayRenderer != null)
        {
            overlayRenderer.sortingLayerID = mySpriteRenderer.sortingLayerID;
            overlayRenderer.sortingOrder = mySpriteRenderer.sortingOrder + 2;
        }

        // 초기 알파값 0으로 숨기기
        Color color = overlayRenderer.color;
        color.a = 0f;
        overlayRenderer.color = color;

        AdjustScale();
    }

    private void AdjustScale()
    {
        if (overlayTrans == null) return;

        Vector3 parentScale = transform.localScale;

        // 0으로 나누는 예외 상황(Division by zero) 방지
        float safeX = Mathf.Approximately(parentScale.x, 0f) ? 1f : parentScale.x;
        float safeY = Mathf.Approximately(parentScale.y, 0f) ? 1f : parentScale.y;
        float safeZ = Mathf.Approximately(parentScale.z, 0f) ? 1f : parentScale.z;

        // 부모 스케일 * 자식 스케일 = 1 이 되도록 역산 (1 / 부모스케일)
        overlayTrans.localScale = new Vector3(1f / safeX, 1f / safeY, 1f / safeZ);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision is BoxCollider2D && overlayRenderer != null)
        {
            SpriteRenderer playerRenderer = collision.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                overlayRenderer.sprite = playerRenderer.sprite;
                overlayRenderer.flipX = playerRenderer.flipX;
                overlayRenderer.flipY = playerRenderer.flipY;
                overlayRenderer.transform.position = playerRenderer.transform.position;

                Color color = playerRenderer.color;
                color.a = overlayAlpha; // 반투명도 설정
                overlayRenderer.color = color;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && overlayRenderer != null)
        {
            Color color = overlayRenderer.color;
            color.a = 0f;
            overlayRenderer.color = color;
        }
    }
}