using System;
using System.Collections;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;

    public bool IsMoving { get; private set; }

    // 코루틴 실행 및 상태 보존을 위한 변수
    private IEnumerator moveRoutineState;
    private Coroutine runner;

    public void MoveTo(Vector3 targetPosition, Action onComplete = null)
    {
        // 이동 중이 아닐 때만 새 이동 시작
        if (!IsMoving)
        {
            moveRoutineState = CoMoveTo(targetPosition, onComplete);
            runner = StartCoroutine(ResumeRoutine());
        }
    }

    private void OnEnable()
    {
        // 끄기 전에 진행 중이던 이동 상태가 남아있다면 이어받아 재실행
        if (moveRoutineState != null)
        {
            runner = StartCoroutine(ResumeRoutine());
        }
    }

    private void OnDisable()
    {
        // SetActive(false)가 되면 실행 중인 러너만 정지 (상태값 _moveRoutineState는 보존됨)
        if (runner != null)
        {
            StopCoroutine(runner);
            runner = null;
        }
    }

    // 멈춘 위치부터 코루틴을 이어주는 래퍼(Wrapper)
    private IEnumerator ResumeRoutine()
    {
        while (moveRoutineState != null && moveRoutineState.MoveNext())
        {
            yield return moveRoutineState.Current;
        }
    }

    private IEnumerator CoMoveTo(Vector3 targetPosition, Action onComplete)
    {
        IsMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPosition; // 위치 오차 정정
        IsMoving = false;

        // 이동 코루틴이 완전히 끝나면 실행 상태 정리
        moveRoutineState = null;
        runner = null;

        onComplete?.Invoke();
    }
}