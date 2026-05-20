using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Target Tracking")]
    [SerializeField] private Transform privateTarget; // 플레이어 Transform
    [SerializeField] private float privateSmoothSpeed = 0.125f;
    [SerializeField] private Vector3 privateOffset = new Vector3(0, 0, -10f);

    [Header("Free Move (Build Mode)")]
    [SerializeField] private float privateMoveSpeed = 20f;

    private bool privateIsFreeMode = false;
    private Vector3 privateCurrentVelocity = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (privateIsFreeMode || privateTarget == null)
        {
            HandleFreeMove();
        }
        else
        {
            HandleTargetTracking();
        }
    }

    // 1. 플레이어 추적 로직 (SmoothDamp 활용)
    private void HandleTargetTracking()
    {
        Vector3 targetPosition = privateTarget.position + privateOffset;
        // SmoothDamp는 Lerp보다 가속/감속이 붙어 훨씬 부드럽습니다.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref privateCurrentVelocity, privateSmoothSpeed);
    }

    // 2. 건설 모드 시 자유 이동 로직 (WASD)
    private void HandleFreeMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, v, 0).normalized;
        transform.position += moveDir * privateMoveSpeed * Time.deltaTime;
    }

    // 모드 전환 함수
    public void SetFreeMode(bool isFree)
    {
        privateIsFreeMode = isFree;

        // 프리 모드로 전환될 때 현재 속도 초기화 (튕김 방지)
        if (isFree) privateCurrentVelocity = Vector3.zero;
    }
}