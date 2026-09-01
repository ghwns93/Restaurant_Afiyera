using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class NonEffectObjectSorting : MonoBehaviour
{
    private Tilemap targetTilemap;

    private void Start()
    {
        targetTilemap = BuildManager.Instance.PrivateTargetTilemap;
        SetFenceOrder();
    }

    public void SetFenceOrder()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in spriteRenderers)
        {
            var childTransform = sr.transform;

            int originalSortingOrder = sr.sortingOrder;

            //Vector3 footPos = childTransform.position;
            //Vector3Int cellPos = targetTilemap.WorldToCell(footPos);

            sr.sortingOrder = Mathf.RoundToInt(-(childTransform.position.y) * 100 - 150) + originalSortingOrder;
        }
    }
}
