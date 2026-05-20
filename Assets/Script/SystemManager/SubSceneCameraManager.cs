using UnityEngine;

public class SubSceneCameraManager : MonoBehaviour
{
    private Camera myCamera;
    private Camera beforeSceneCamera;

    private AudioListener mainListener;

    void Awake()
    {
        myCamera = GetComponent<Camera>();

        // 1. 모든 카메라를 찾아서 '나'를 제외한 다른 MainCamera들을 비활성화
        Camera[] allCameras = Camera.allCameras;
        foreach (var cam in allCameras)
        {
            if (cam != myCamera && cam.CompareTag("MainCamera"))
            {
                beforeSceneCamera = cam;
                // 메인 카메라를 끄거나, 우선순위(Depth)를 낮춤
                cam.gameObject.SetActive(false);
            }
        }

        // 2. AudioListener 중복 해결 (씬에 하나만 활성화되어야 함)
        mainListener = FindFirstObjectByType<AudioListener>();
        AudioListener myListener = GetComponent<AudioListener>();

        if (mainListener != null && myListener != null && mainListener != myListener)
        {
            mainListener.enabled = false; // 메인 리스너를 끄고 내 것을 사용
        }
    }

    // 서브씬이 언로드될 때 메인 카메라를 다시 켜주는 옵션 (필요 시)
    void OnDestroy()
    {
        if (beforeSceneCamera != null) beforeSceneCamera.gameObject.SetActive(true);

        if (mainListener != null) mainListener.enabled = true;
    }
}