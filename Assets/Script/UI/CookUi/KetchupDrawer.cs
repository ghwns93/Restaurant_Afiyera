using UnityEngine;
using System.Collections.Generic;

public class KetchupDrawer : MonoBehaviour
{
    [Header("케첩 선 프리팹")]
    public GameObject ketchupLinePrefab;

    private LineRenderer currentLine;
    private List<Vector3> linePoints = new List<Vector3>();

    [Header("설정")]
    [Tooltip("이 거리 이상 마우스가 움직여야 선에 점을 추가합니다 (부드러움 및 최적화)")]
    public float minDistance = 0.05f;
    [Tooltip("케첩이 UI 바로 앞에 그려지도록 카메라로부터 떨어진 거리 조절")]
    public float zOffset = 5.0f;

    void Update()
    {
        // 1. 마우스를 처음 누를 때 -> 새로운 케첩 줄기 생성
        if (Input.GetMouseButtonDown(0))
        {
            CreateNewLine();
        }

        // 2. 마우스를 누른 채 드래그할 때 -> 선 연장
        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();

            // 이전 점과 현재 마우스 위치의 거리를 비교해서 일정 이상 움직였을 때만 점 추가
            if (Vector3.Distance(mouseWorldPos, linePoints[linePoints.Count - 1]) > minDistance)
            {
                UpdateLine(mouseWorldPos);
            }
        }

        // 3. 마우스에서 손을 뗄 때 -> 현재 그리던 선 해제
        if (Input.GetMouseButtonUp(0))
        {
            currentLine = null;
        }
    }

    void CreateNewLine()
    {
        // 프리팹 생성
        GameObject newLineGo = Instantiate(ketchupLinePrefab, Vector3.zero, Quaternion.identity);
        currentLine = newLineGo.GetComponent<LineRenderer>();

        linePoints.Clear();
        Vector3 startPos = GetMouseWorldPosition();

        // LineRenderer 초기 세팅 (시작점과 끝점을 같은 위치로 2개 생성)
        linePoints.Add(startPos);
        linePoints.Add(startPos);

        currentLine.positionCount = 2;
        currentLine.SetPosition(0, startPos);
        currentLine.SetPosition(1, startPos);
    }

    void UpdateLine(Vector3 newPoint)
    {
        linePoints.Add(newPoint);

        // LineRenderer의 점 개수를 늘려주고 마지막 자리에 새 좌표 대입
        currentLine.positionCount = linePoints.Count;
        currentLine.SetPosition(linePoints.Count - 1, newPoint);
    }

    // 마우스의 스크린 좌표를 2D 월드(Camera Space) 좌표로 변환하는 함수
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        // Screen Space - Camera 환경에서는 Z값(카메라와의 거리)이 반드시 필요합니다.
        // Canvas가 카메라와 떨어진 거리 만큼 Z값을 주어야 캔버스 평면에 정확히 그려집니다.
        mouseScreenPos.z = zOffset;

        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}