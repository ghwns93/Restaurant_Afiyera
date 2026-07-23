using System;
using System.Collections;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    public bool IsMoving { get; private set; }
    public void MoveTo(Vector3 targetPosition, Action onComplete = null)
    {
        if (!IsMoving)
        {
            StartCoroutine(CoMoveTo(targetPosition, onComplete));
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

        onComplete?.Invoke();
    }
}