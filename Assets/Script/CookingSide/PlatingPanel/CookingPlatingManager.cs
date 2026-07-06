using System.Collections.Generic;
using UnityEngine;

public class CookingPlatingManager : MonoBehaviour
{
    public static CookingPlatingManager Instance { get; private set; }

    [SerializeField] List<CookingPlate> _plates;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public CookingPlate FindPlate(PlateType type)
    {
        return _plates.Find((x) => x.Type == type);
    }

    public float GetOverlapRatioIoU(RectTransform rectA, RectTransform rectB)
    {
        Rect worldRectA = GetWorldRect(rectA);
        Rect worldRectB = GetWorldRect(rectB);
        Rect overlap = GetIntersection(worldRectA,worldRectB);

        float overlapArea = overlap.width * overlap.height;
        float areaA = GetWorldRect(rectA).width * GetWorldRect(rectA).height;
        float areaB = GetWorldRect(rectB).width * GetWorldRect(rectB).height;

        float unionArea = areaA + areaB - overlapArea;

        if (unionArea == 0f) return 0f;
        return overlapArea / unionArea;
    }

    private Rect GetWorldRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;

        return new Rect(xMin,yMin,xMax-xMin,yMax-yMin);
    }

    public Rect GetIntersection(Rect a, Rect b)
    {
        float xMin = Mathf.Max(a.xMin, b.xMin);
        float yMin = Mathf.Max (a.yMin, b.yMin);
        float xMax = Mathf.Min(a.xMax, b.xMax);
        float yMax = Mathf.Min(a.yMax, b.yMax);

        if (xMax <= xMin || yMax <= yMin)
            return Rect.zero;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}
public enum PlateType { None, Plate, Cup, Dish};