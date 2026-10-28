using UnityEngine;

public class StarEffectItem : MonoBehaviour
{
    private float moveSpeed;
    private float lifeTime;
    private float currentAge;
    private SpriteRenderer spriteRenderer;

    public void Initialize(float speed, float maxLife)
    {
        moveSpeed = speed;
        lifeTime = maxLife;
        currentAge = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        currentAge += Time.deltaTime;

        // 위로 이동 (Y축 방향 플러스)
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 서서히 투명해지기 (Fade Out)
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Clamp01(1f - (currentAge / lifeTime));
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        // 일정 시간이 지나면 파괴
        if (currentAge >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}