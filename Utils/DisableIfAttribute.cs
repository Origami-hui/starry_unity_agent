using UnityEngine;

public class DisableIfAttribute : PropertyAttribute
{
    public string ConditionField { get; private set; }
    public bool TargetValue { get; private set; }

    public DisableIfAttribute(string conditionField, bool targetValue = false)
    {
        ConditionField = conditionField;
        TargetValue = targetValue;
    }
}