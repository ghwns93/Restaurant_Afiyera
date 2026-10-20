using UnityEngine;

// 1. 오브젝트의 대분류 타입 (나중에 종류를 쉽게 추가할 수 있습니다)
public enum GardenObjectType
{
    Statue, // 석상
}

[CreateAssetMenu(fileName = "NewGardenObjectData", menuName = "Garden/Garden Object Data")]
public class GardenObjectData : ItemData
{
    [Header("Basic Info")]
    public GameObject objectPrefab; // 필드(빨간 위치)에 생성될 실제 프리팹

    [Header("Category & Matching")]
    public GardenObjectType objectType;       // 이 오브젝트의 종류 (Statue, Fence 등)
    public ResourceType resourceType;         // 해당되는 자원 타입 (Rock, Water 등)
}