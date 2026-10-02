using UnityEditor;
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string fieldName;
    public int[] enumIndexes;   // null이면 bool 모드

    // bool 조건: [ShowIf("isInteractionNpc")]
    public ShowIfAttribute(string boolName)
    {
        fieldName = boolName;
    }

    // enum 조건: [ShowIf("npcType", NpcType.Merchant)]
    // 여러 값 중 하나라도 일치하면 표시: [ShowIf("npcType", NpcType.Merchant, NpcType.Chef)]
    public ShowIfAttribute(string enumFieldName, params object[] enumValues)
    {
        fieldName = enumFieldName;
        enumIndexes = new int[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
            enumIndexes[i] = (int)enumValues[i];
    }
}