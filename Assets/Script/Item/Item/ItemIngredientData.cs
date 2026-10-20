using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Ingredients")]
public class ItemIngredientData : ItemData
{
    public ResourceType resourceType;

    public bool isUnlocked;

    public float minPriceMultiplier = 0.5f; // 최소 가격 배율
    public float maxPriceMultiplier = 1.5f; // 최대 가격 배율
}
