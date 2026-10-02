using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NpcBasicInfo), true)]
[CanEditMultipleObjects]
public class ShowIfEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var prop = serializedObject.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (prop.propertyPath == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(prop);
                continue;
            }

            if (!IsVisible(prop)) continue;   // 컬렉션 전체를 통째로 건너뜀
            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    bool IsVisible(SerializedProperty prop)
    {
        return IsVisibleByName(prop.name, 0);
    }

    bool IsVisibleByName(string fieldName, int depth)
    {
        if (depth > 10) return true;   // 순환 참조 방지

        var field = FindField(target.GetType(), fieldName);
        if (field == null) return true;

        var attr = field.GetCustomAttribute<ShowIfAttribute>();
        if (attr == null) return true;

        // 조건 필드 자체가 숨겨져 있으면 이 필드도 숨김 (연쇄)
        if (!IsVisibleByName(attr.fieldName, depth + 1)) return false;

        var condProp = serializedObject.FindProperty(attr.fieldName);
        if (condProp == null) return true;

        switch (condProp.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return condProp.boolValue;

            case SerializedPropertyType.Enum:
                if (attr.enumIndexes == null) return true;

                var enumField = FindField(target.GetType(), attr.fieldName);
                if (enumField == null) return true;

                var values = Enum.GetValues(enumField.FieldType);
                if (condProp.enumValueIndex < 0 || condProp.enumValueIndex >= values.Length)
                    return false;

                int current = Convert.ToInt32(values.GetValue(condProp.enumValueIndex));
                return Array.IndexOf(attr.enumIndexes, current) >= 0;

            default:
                return true;
        }
    }

    static FieldInfo FindField(System.Type type, string name)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        while (type != null)
        {
            var f = type.GetField(name, flags | BindingFlags.DeclaredOnly);
            if (f != null) return f;
            type = type.BaseType;
        }
        return null;
    }
}