using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "OrderGame/CustomerData")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;
    public FoodData targetFood;
    public List<FoodKeyword> hintKeywords;

    [Header("JSON Dialogue Source")]
    public TextAsset dialogueJsonFile; // JSON 파일을 Inspector에서 할당

    // JSON 파일 데이터를 파싱하여 대화 리스트 반환
    public List<OrderDialogueData> GetDialogues()
    {
        if (dialogueJsonFile != null)
        {
            OrderDialogueListWrapper wrapper = JsonUtility.FromJson<OrderDialogueListWrapper>(dialogueJsonFile.text);
            return wrapper.dialogues;
        }

        Debug.LogWarning($"{customerName}의 대화 JSON 파일이 할당되지 않았습니다.");
        return new List<OrderDialogueData>();
    }
}
