using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int id;             // 고유 ID (딕셔너리 관리용)
    public string itemName;       // 이름
    public Sprite icon;           // UI에 표시할 아이콘
    [TextArea] public string description; // 설명

    public BuffEffect buffEffect; // 일꾼 버프
}