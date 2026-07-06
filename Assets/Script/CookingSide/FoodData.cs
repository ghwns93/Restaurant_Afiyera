using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Cooking/FoodData")]
public class FoodData : ScriptableObject
{
    public int id;
    public string foodName;
    public int seasoningTier;
    public CookType cookType;
    public PlateType plateType;
    public int[] mat;
    public Sprite iconPlated;
    public Sprite iconCooked;
    [TextArea] public string description;
}
