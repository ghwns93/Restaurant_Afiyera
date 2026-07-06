using UnityEngine;

public class CookingTempSeasoningManager : MonoBehaviour
{
    public static CookingTempSeasoningManager Instance { get; private set; }

    [SerializeField] private int[] _seasoningCnts;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public int GetSeasoningCount(int tier)
    {
        return _seasoningCnts[tier];
    }

    public void DiscountSeasning(int tier, int count)
    {
        _seasoningCnts[tier] -= count;
    }
}
