using UnityEngine;

public class WebLinkManager : MonoBehaviour
{
    [SerializeField] private string targetUrl = "https://www.example.com";

    // 버튼의 OnClick 이벤트에 연결할 함수
    public void OpenTargetURL()
    {
        if (!string.IsNullOrEmpty(targetUrl))
        {
            Application.OpenURL(targetUrl);
        }
    }

    // 특정 URL을 직접 매개변수로 전달받아 열고 싶을 때 사용
    public void OpenCustomURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }
}