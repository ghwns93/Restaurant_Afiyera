using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    private void OnDisable()
    {
        if (PlayerInfoLoadManager.Instance != null && Camera.main != null)
        {
            PlayerData currentData = new PlayerData
            {
                isNew = 1,
                lastPosition = transform.position,
                lastCameraPosition = Camera.main.transform.position
            };

            //Debug.Log($"Player position saved: {currentData.lastPosition}");

            PlayerInfoLoadManager.Instance.NewDataStructure(currentData);
        }
    }
}
