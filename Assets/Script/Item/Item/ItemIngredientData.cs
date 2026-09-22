using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Ingredients")]
public class ItemIngredientData : ItemData
{
    public int cookTime;                    // 요리 시간
    public Sprite refineIcon;               // 손질된 재료 아이콘
    public BuffEffect buffEffect;           // 일꾼 버프

    public float minPriceMultiplier = 0.5f; // 최소 가격 배율
    public float maxPriceMultiplier = 1.5f; // 최대 가격 배율
}
