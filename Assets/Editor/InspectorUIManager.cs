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

        // ��������Ƿ�����
        bool shouldDisable = conditionProp.boolValue == disableAttr.TargetValue;

        // ����GUI״̬
        bool prevGUIState = GUI.enabled;
        GUI.enabled = !shouldDisable;

        // ��������
        EditorGUI.PropertyField(position, property, label);

        // �ָ�GUI״̬
        GUI.enabled = prevGUIState;
    }
}
