using UnityEngine;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    private bool isMoving = false;
    private Vector3 targetPosition;
    [SerializeField] private float gridSize = 1.0f;

    void Start()
    {
        // 시작할 때 위치를 가장 가까운 그리드(정수 좌표)에 딱 붙임
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            0
        );
    }

    void Update()
    {
        // 이동 중이 아닐 때만 입력을 받음
        if (!isMoving)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal != 0)
            {
                StartCoroutine(MovePlayer(new Vector3(horizontal * gridSize, 0, 0)));
            }
            else if (vertical != 0)
            {
                StartCoroutine(MovePlayer(new Vector3(0, vertical * gridSize, 0)));
            }
        }
    }

    IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        targetPosition = transform.position + direction;

        // 목표 지점에 도달할 때까지 보간 이동
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // 위치 보정
        isMoving = false;
    }
}