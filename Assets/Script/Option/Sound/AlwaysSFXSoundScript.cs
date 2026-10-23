using UnityEngine;

public class AlwaysSFXSoundScript : MonoBehaviour
{
    [SerializeField] private SoundName soundName;

    void Start()
    {
        // 게임 시작 시 분수 위치에 상시형 사운드 등록 (가까우면 최대 음량, 멀어지면 작아짐)
        SFXPlayer.RegisterAmbient(soundName, transform.position);
    }
}
