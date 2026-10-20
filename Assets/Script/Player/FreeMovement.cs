using UnityEngine;
using Spine.Unity;

public class FreeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSpeedRatio = 1.5f; // Shift를 누를 때 곱해지는 배율

    [Header("Spine Animation Settings")]
    [SerializeField] private SkeletonAnimation WKeySpine; // W 키 입력 시 사용할 정면 스파인 컴포넌트
    [SerializeField] private SkeletonAnimation SKeySpine; // 기본으로 사용할 스파인 컴포넌트

    [Header("Flip Settings (좌우반전 방향이 안 맞을 때 체크)")]
    [SerializeField] private bool invertWKeyFlip = false; // WKeySpine 좌우반전 반대로 뒤집기
    [SerializeField] private bool invertSKeyFlip = false; // SKeySpine 좌우반전 반대로 뒤집기

    [SpineAnimation][SerializeField] private string idleAnim = "idle";
    [SpineAnimation][SerializeField] private string walkAnim = "walk";

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool canMove = true;

    // 마지막으로 활성화된 스파인 상태를 기억하는 변수
    private bool isWSpineActive = false;

    // 각 스파인별 현재 재생 중인 애니메이션 기억용 변수 (씹힘 방지)
    private string wCurrentAnimation = "";
    private string sCurrentAnimation = "";

    private void OnEnable()
    {
        SystemController.OnSystemStateChanged += HandleSystemState;
    }

    private void OnDisable()
    {
        SystemController.OnSystemStateChanged -= HandleSystemState;
    }

    private void HandleSystemState(bool isPaused)
    {
        canMove = isPaused;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (SKeySpine == null)
            SKeySpine = GetComponentInChildren<SkeletonAnimation>();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            UpdateAnimationState(0f, 0f);
            return;
        }

        // WASD 입력 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(h, v * (h != 0 ? 0.5f : 1f));

        if (moveInput.sqrMagnitude > 1)
        {
            moveInput.Normalize();
        }

        // 애니메이션 및 방향 상태 결정
        UpdateAnimationState(h, v);
    }

    void FixedUpdate()
    {
        float ratio = 1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isRunning)
        {
            ratio = moveSpeedRatio;
        }

        // 물리 기반 이동 처리
        rb.linearVelocity = moveInput * moveSpeed * ratio;
    }

    private void UpdateAnimationState(float h, float v)
    {
        // 1. A와 D 키 입력에 따른 좌우 반전 처리
        if (h > 0)
        {
            SetFacingDirection(true);  // D 키 (오른쪽)
        }
        else if (h < 0)
        {
            SetFacingDirection(false); // A 키 (왼쪽)
        }

        // 2. W나 S 키가 입력될 때만 스파인 종류를 변경
        if (v > 0)
        {
            isWSpineActive = true;
        }
        else if (v < 0)
        {
            isWSpineActive = false;
        }

        // 현재 유지해야 할 스파인 활성화
        ShowSpine(isWSpineActive);
        SkeletonAnimation activeSpine = isWSpineActive ? WKeySpine : SKeySpine;

        // 3. 이동 입력이 거의 없으면 (멈췄을 때) -> Idle 애니메이션
        if (moveInput.sqrMagnitude < 0.01f)
        {
            SetAnimation(activeSpine, idleAnim, true);
            if (activeSpine != null) activeSpine.timeScale = 1f;
            return;
        }

        // 4. 이동 중일 때 속도 배율 결정 (Shift 여부)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float timeScale = isRunning ? moveSpeedRatio : 1f;

        // 5. 활성화된 스파인에 걷기 애니메이션 적용
        SetAnimation(activeSpine, walkAnim, true);
        if (activeSpine != null) activeSpine.timeScale = timeScale;
    }

    // WKeySpine과 SKeySpine 중 어떤 것을 켤지 결정하는 함수
    private void ShowSpine(bool useW)
    {
        if (WKeySpine != null) WKeySpine.gameObject.SetActive(useW);
        if (SKeySpine != null) SKeySpine.gameObject.SetActive(!useW);
    }

    // 좌우 반전 방향 설정
    private void SetFacingDirection(bool facingRight)
    {
        float baseSign = facingRight ? 1f : -1f;
        ApplyScaleX(baseSign);
    }

    // 각 스파인별 개별 반전 옵션(invert)을 적용하여 X 스케일 설정
    private void ApplyScaleX(float baseSign)
    {
        if (WKeySpine != null)
        {
            Vector3 scaleW = WKeySpine.transform.localScale;
            float finalSignW = invertWKeyFlip ? -baseSign : baseSign;
            scaleW.x = Mathf.Abs(scaleW.x) * finalSignW;
            WKeySpine.transform.localScale = scaleW;
        }

        if (SKeySpine != null)
        {
            Vector3 scaleS = SKeySpine.transform.localScale;
            float finalSignS = invertSKeyFlip ? -baseSign : baseSign;
            scaleS.x = Mathf.Abs(scaleS.x) * finalSignS;
            SKeySpine.transform.localScale = scaleS;
        }
    }

    // 스파인 개별 애니메이션 중복 방지 및 세팅
    private void SetAnimation(SkeletonAnimation spine, string animName, bool loop)
    {
        if (spine == null) return;
        if (string.IsNullOrEmpty(animName)) return;

        if (spine == WKeySpine)
        {
            if (wCurrentAnimation == animName) return;
            wCurrentAnimation = animName;
        }
        else if (spine == SKeySpine)
        {
            if (sCurrentAnimation == animName) return;
            sCurrentAnimation = animName;
        }

        spine.AnimationState.SetAnimation(0, animName, loop);
    }
}