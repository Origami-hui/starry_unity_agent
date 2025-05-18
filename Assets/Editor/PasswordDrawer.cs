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
            // 使用PasswordField绘制字符串
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
        else
        {
            EditorGUI.HelpBox(position, "[Password] 只能用于string类型", MessageType.Error);
        }
    }
}
#endif