using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int id;                          // 고유 ID (딕셔너리 관리용)
    [SerializeField] private string itemName;                 // 이름
    [SerializeField] private Sprite icon;                     // UI에 표시할 아이콘
    [SerializeField] private ItemType itemType;               // 아이템 타입
    [SerializeField] private bool isStackable;                // 중첩 가능 여부
    [SerializeField]  private int maxStackCount = 99;          // 최대 중첩 수량
    [TextArea] public string description;   // 설명

    public int Id => id;
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public ItemType ItemType => itemType;
    public bool IsStackable => isStackable;
    public int MaxStackCount => maxStackCount;
    public string Description => description;
}

public enum ItemType
{
    Ingredient, // 요리 재료
    Tool,       // 도구 (곡괭이, 칼 등)
    Special,    // 특수 아이템
    Flavoring   // 양념 등
}