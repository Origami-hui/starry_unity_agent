#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PasswordAttribute))]
public class PasswordDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            // ʹ��PasswordField�����ַ���
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
        else
        {
            EditorGUI.HelpBox(position, "[Password] ֻ������string����", MessageType.Error);
        }
    }
}
#endif