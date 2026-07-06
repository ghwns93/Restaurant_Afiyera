using UnityEngine;

public class TempCookAndNightCookScene : MonoBehaviour
{
    public void CookEnd()
    {
        RestaurantManager.Instance.CloseRestaurnat();
    }

    public void NightCookEnd()
    {
        RestaurantManager.Instance.CloseNightRestaurnat();
    }
}
