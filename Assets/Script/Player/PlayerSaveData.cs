using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    private void OnDisable()
    {
        if (PlayerInfoLoadManager.Instance != null && Camera.main != null)
        {
            var mainCamera = Camera.main;

            int cameraBound = mainCamera.GetComponent<SimpleCameraConfiner>().GetNowBound();

            PlayerData currentData = new PlayerData
            {
                isNew = 1,
                lastPosition = transform.position,
                lastCameraPosition = mainCamera.transform.position,
                lastCameraBoundIndex = cameraBound
            };

            //Debug.Log($"Player position saved: {currentData.lastPosition}");

            PlayerInfoLoadManager.Instance.NewDataStructure(currentData);
        }
    }
}
