using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    private void OnDisable()
    {
        if (PlayerInfoLoadManager.Instance != null)
        {
            PlayerData currentData = new PlayerData
            {
                lastPosition = transform.position,
                lastCameraPosition = Camera.main.transform.position
            };

            //Debug.Log($"Player position saved: {currentData.lastPosition}");

            PlayerInfoLoadManager.Instance.NewDataStructure(currentData);
        }
    }
}
