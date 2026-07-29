using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 선택지 버튼 프리팹에 붙이는 스크립트.
// 프리팹 구성: Button + 배경 Image + 자식으로 TMP_Text
[RequireComponent(typeof(Button))]
public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private Button button;
    private Action onClick;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void Setup(string text, Action onClickAction)
    {
        label.text = text;
        onClick = onClickAction;
    }
}
