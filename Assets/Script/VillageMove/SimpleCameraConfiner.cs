using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraConfiner : MonoBehaviour
{
    [Header("현재 카메라 제한 영역 (BoxCollider2D 또는 PolygonCollider2D)")]
    [SerializeField] private Collider2D currentBoundsCollider;

    [SerializeField] private List<Collider2D> boundAreas;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // 카메라의 추적/이동 스크립트가 실행된 후(LateUpdate) 경계를 강제로 제한합니다.
        ClampCameraPosition();
    }

    public int GetNowBound()
    {
        for(int i = 0; i < boundAreas.Count; i++)
        {
            if (boundAreas[i] == currentBoundsCollider)
            {
                return i;
            }
        }

        return -1; // 현재 바운드가 리스트에 없으면 -1 반환
    }

    public void SetBoundByIndex(int index)
    {
        if (index >= 0 && index < boundAreas.Count)
        {
            currentBoundsCollider = boundAreas[index];
            ClampCameraPosition();
        }
        else
        {
            Debug.LogWarning("Invalid index for camera bounds.");
        }
    }

    public void SetBounds(Collider2D newBounds)
    {
        currentBoundsCollider = newBounds;
        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        if (currentBoundsCollider == null) return;

        // 1. 카메라 시야(Orthographic Viewport)의 반폭과 반높이 구하기
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        // 2. 콜라이더의 바운딩 박스(AABB) 좌표 구하기
        Bounds bounds = currentBoundsCollider.bounds;

        // 3. 카메라 위치가 바운더리 밖으로 나가지 않도록 제한(Clamp)
        Vector3 pos = transform.position;

        // 맵의 크기가 카메라 화면보다 큰 경우에만 위치 제한
        if (bounds.size.x >= camHalfWidth * 2f)
        {
            float minX = bounds.min.x + camHalfWidth;
            float maxX = bounds.max.x - camHalfWidth;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }
        else
        {
            // 맵이 카메라 시야보다 작은 경우 맵의 중앙에 고정
            pos.x = bounds.center.x;
        }

        if (bounds.size.y >= camHalfHeight * 2f)
        {
            float minY = bounds.min.y + camHalfHeight;
            float maxY = bounds.max.y - camHalfHeight;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
        else
        {
            pos.y = bounds.center.y;
        }

        transform.position = pos;
    }
}