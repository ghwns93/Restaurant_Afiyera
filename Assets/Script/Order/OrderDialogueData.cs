// 1. JSON 직렬화가 가능한 대화 데이터 클래스
using System.Collections.Generic;

[System.Serializable]
public class OrderDialogueData
{
    public bool isCustomer; // true: 손님, false: 유저
    public string message;
}

// 2. JSON 파싱을 위한 Wrapper 클래스
[System.Serializable]
public class OrderDialogueListWrapper
{
    public List<OrderDialogueData> dialogues;
}