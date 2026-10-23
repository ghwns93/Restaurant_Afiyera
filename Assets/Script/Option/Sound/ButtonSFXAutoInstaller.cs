#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXAutoInstaller : EditorWindow
{
    [MenuItem("Tools/모든 버튼에 사운드 스크립트 자동 추가")]
    public static void ApplyToAllButtons()
    {
        // 유니티 6 최신 API 기준 씬의 모든 버튼 탐색
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var btn in buttons)
        {
            // 이미 붙어있지 않다면 추가
            if (btn.GetComponent<UIButtonSoundHandler>() == null)
            {
                // 일반 AddComponent가 아닌 Undo.AddComponent를 써야 에디터 저장(Dirty) 처리가 정상적으로 됨
                Undo.AddComponent<UIButtonSoundHandler>(btn.gameObject);
                count++;
            }
        }

        if (count > 0)
        {
            // 현재 씬이 수정되었음을 유니티 에디터에 명시 (저장 필요 상태로 만듦)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[SoundSystem] 총 {count}개의 새로운 버튼에 UI 사운드 스크립트가 자동 장착되었습니다!");
        }
        else
        {
            Debug.Log("[SoundSystem] 새로 추가할 버튼이 없습니다. (모두 이미 적용되어 있음)");
        }
    }
}
#endif