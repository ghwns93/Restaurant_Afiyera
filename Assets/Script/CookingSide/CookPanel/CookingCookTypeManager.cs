using System.Collections.Generic;
using UnityEngine;

public class CookingCookTypeManager : MonoBehaviour
{
    public static CookingCookTypeManager Instance { get; private set; }

    [SerializeField] private List<FoodData> _foodList;
    [SerializeField] private List<CookTypeImage> _cookTypeImagelist;
    [SerializeField] private GameObject _foodPrefab;

    public GameObject FoodPrefab { get { return _foodPrefab; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<FoodData> GetAllFoodData()
    { 
        return _foodList; 
    }

    public FoodData GetFoodDataById(int id)
    {
        return _foodList.Find(x => x.id == id);
    }

    public FoodData GetFoodDataByMat(int mat1, int mat2,CookType cookType)
    {
        return _foodList.Find(x => x.mat.Length >= 2 &&
        ((x.mat[0] == mat1 && x.mat[1] == mat2) ||
        (x.mat[0] == mat2 && x.mat[1] == mat1)) &&
        x.cookType == cookType);
    }

    public Sprite GetCookTypeImage(CookType cookType)
    {
        return _cookTypeImagelist.Find(x => x.cookType == cookType).cookSprite;
    }
}

public enum CookType { None, Fire, Ice, Time};

[System.Serializable]
public struct CookTypeImage
{
    public CookType cookType;
    public Sprite cookSprite;
}