using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisableIfAttribute))]
public class InspectorUIManager : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisableIfAttribute disableAttr = (DisableIfAttribute)attribute;
        SerializedObject serializedObject = property.serializedObject;
        SerializedProperty conditionProp = serializedObject.FindProperty(disableAttr.ConditionField);

        // 检查条件是否满足
        bool shouldDisable = conditionProp.boolValue == disableAttr.TargetValue;

        // 设置GUI状态
        bool prevGUIState = GUI.enabled;
        GUI.enabled = !shouldDisable;

        // 绘制属性
        EditorGUI.PropertyField(position, property, label);

        // 恢复GUI状态
        GUI.enabled = prevGUIState;
    }
}
