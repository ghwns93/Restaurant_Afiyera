using UnityEngine;

public class TempNightOKScript : MonoBehaviour
{
    public void NightRestaurantOK()
    {
        TimeEvents.OnNightRestaurant?.Invoke();
    }
}
