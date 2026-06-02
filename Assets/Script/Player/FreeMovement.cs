using UnityEngine;

public class FreeMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 인스펙터에서 조절 가능
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool canMove = true;

    private void OnEnable()
    {
        // 이벤트 구독
        SystemController.OnSystemStateChanged += HandleSystemState;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 해제 필수!
        SystemController.OnSystemStateChanged -= HandleSystemState;
    }

    private void HandleSystemState(bool isPaused)
    {
        canMove = isPaused;
    }

    void Start()
    {
        // Rigidbody2D 설정 (Gravity Scale은 0으로 설정하세요)
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove) return;

        // WASD 입력 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(h, v * (h != 0 ? 0.5f : 1f));

        // 대각선 이동 시 속도가 빨라지지 않도록 정규화 (Normalize)
        if (moveInput.sqrMagnitude > 1)
        {
            moveInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        // 물리 기반 이동 처리
        rb.linearVelocity = moveInput * moveSpeed;
    }
}