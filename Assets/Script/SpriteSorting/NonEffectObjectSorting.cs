using UnityEngine;
using UnityEngine.Rendering;

public class NonEffectObjectSorting : MonoBehaviour
{
    private void Start()
    {
        SetFenceOrder();
    }

    public void SetFenceOrder()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child != null)
            {
                var spriteRenderers = child.GetComponent<SpriteRenderer>();

                if(spriteRenderers != null)
                {
                    spriteRenderers.sortingOrder = Mathf.RoundToInt(-(child.position.x + child.position.y) * 100 - 150);
                }
            }
        }
    }
}
