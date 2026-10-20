using System.Collections.Generic;
using UnityEngine;

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance;

    [Header("Place Points (Fixed)")]
    public List<Transform> placePoints;

    [Header("Database (All Garden Objects)")]
    [System.NonSerialized]
    public List<ItemSlot> availableGardenObjects;

    // 실제 월드(빨간 위치)에 생성된 오브젝트들을 담아두는 딕셔너리
    private Dictionary<int, GameObject> spawnedInstances = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadGardenObjects();
        InitializeWorldObjects(); // 씬 진입 시 저장된 데이터를 바탕으로 월드 오브젝트 생성
    }

    private void LoadGardenObjects()
    {
        availableGardenObjects = InventoryManager.Instance.GetSlots(ItemType.Garden);
    }

    // 씬에 처음 들어왔을 때, GardenRewardManager에 저장된 데이터를 읽어와 화면에 뿌려줌
    private void InitializeWorldObjects()
    {
        if (GardenRewardManager.Instance == null) return;

        foreach (var pair in GardenRewardManager.Instance.slotDataMap)
        {
            int slotIndex = pair.Key;
            var savedData = pair.Value;

            if (savedData.selectedObject != null)
            {
                SpawnWorldObject(slotIndex, savedData.selectedObject);
            }
        }
    }

    // UI에서 오브젝트(석상 등)를 변경했을 때 호출
    public void ChangeGardenObject(int slotIndex, GardenObjectData objData)
    {
        // 1. 영구 데이터 매니저에 저장 요청
        if (GardenRewardManager.Instance != null)
        {
            GardenRewardManager.Instance.SetGardenObject(slotIndex, objData);
        }

        // 2. 화면(월드)에 오브젝트 생성/갱신 [이 부분이 빠져있어 생성이 안 되었던 것입니다!]
        SpawnWorldObject(slotIndex, objData);
    }

    // UI에서 재료를 변경했을 때 호출
    public void ChangeMaterial(int slotIndex, ItemIngredientData material)
    {
        if (GardenRewardManager.Instance != null)
        {
            GardenRewardManager.Instance.SetMaterial(slotIndex, material);
        }
    }

    // 실제 월드(빨간 위치)에 프리팹을 생성하는 내부 메서드
    private void SpawnWorldObject(int slotIndex, GardenObjectData objData)
    {
        if (slotIndex < 0 || slotIndex >= placePoints.Count) return;

        Transform targetPoint = placePoints[slotIndex];

        // 기존에 생성된 오브젝트가 있다면 파괴
        if (spawnedInstances.ContainsKey(slotIndex) && spawnedInstances[slotIndex] != null)
        {
            Destroy(spawnedInstances[slotIndex]);
            spawnedInstances.Remove(slotIndex);
        }

        // 새로운 오브젝트 생성
        if (objData != null && objData.objectPrefab != null)
        {
            GameObject newInstance = Instantiate(objData.objectPrefab, targetPoint.position, targetPoint.rotation, targetPoint);
            spawnedInstances[slotIndex] = newInstance;
        }
    }

    public void ReturnVillage()
    {
        SystemController.Instance.SetSystemPause(true);
        SceneController.Instance.LoadSubScene(SceneType.Village);
    }
}