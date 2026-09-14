using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Ingredients")]
public class ItemIngredientData : ItemData
{
    public int cookTime;                    // 요리 시간
    public Sprite refineIcon;               // 손질된 재료 아이콘
    public BuffEffect buffEffect;           // 일꾼 버프
}
