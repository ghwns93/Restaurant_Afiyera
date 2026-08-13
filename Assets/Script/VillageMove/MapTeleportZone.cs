using System.Collections;
using UnityEngine;

public class MapTeleportZone : MonoBehaviour
{
    [Header("이동 관련 설정")]
    [SerializeField] private Transform targetSpawnPoint; // 목적지 스폰 위치
    [SerializeField] private Direction moveDirection;    // 이동 연출 방향

    [Header("카메라 설정")]
    [SerializeField] private Collider2D targetMapBoundingCollider; // 이동할 맵의 영역 Collider

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportSequence(collision.transform));
        }
    }

    private IEnumerator TeleportSequence(Transform player)
    {
        isTeleporting = true;

        // 1. 화면 전환 연출 시작
        yield return ScreenTransitionManager.Instance.PlayTransition(moveDirection, () =>
        {
            // [화면이 완전 검은색으로 가려졌을 때 실행]

            // 플레이어 위치 이동
            player.position = targetSpawnPoint.position;

            // 메인 카메라의 이동 위치를 스폰 지점으로 직접 순간이동 (카메라 추적 보정용)
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Vector3 camPos = mainCam.transform.position;
                camPos.x = targetSpawnPoint.position.x;
                camPos.y = targetSpawnPoint.position.y;
                mainCam.transform.position = camPos;

                // 새로운 맵의 Collider 영역으로 경계 제한 변경
                var confiner = mainCam.GetComponent<SimpleCameraConfiner>();
                if (confiner != null)
                {
                    confiner.SetBounds(targetMapBoundingCollider);
                }
            }
        });

        isTeleporting = false;
    }
}