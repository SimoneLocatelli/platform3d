using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeAttributeDrawer : PropertyDrawer
{
    float min = float.MinValue;
    float max = float.MaxValue;

    private MinMaxRangeAttribute RangeAttribute => (MinMaxRangeAttribute)attribute;

    private static readonly Type SupportedTypeName = typeof(Vector2);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Assert.IsTrue(property.type == SupportedTypeName.Name, $"Property '{property.name}' must be of type '{SupportedTypeName.FullName}', but actual type is '{property.type}'");
        Assert.IsNotNull(RangeAttribute);

        // Log about incorrect configuration
        if (RangeAttribute.min > RangeAttribute.max)
            Debug.LogError($"{nameof(MinMaxRangeAttributeDrawer)} - Property '{label}' - Min limit is greater than max limit");
        else if (RangeAttribute.min > RangeAttribute.max)
            Debug.LogWarning($"{nameof(MinMaxRangeAttributeDrawer)} - Property '{label}' - Min limit is equal to max limit");

        

        // Get Current Values
         min = property.vector2Value.x;
         max = property.vector2Value.y;

        if(property.vector2Value == Vector2.zero)
        {
            // Ensure values are in correct format (rounded and between valid ranges)
           // min = RangeAttribute.min;
           // max = RangeAttribute.max;
        }


        // Draw property label
        var labelPosition = new Rect(position.x, position.y, position.width, position.height);
        position = EditorGUI.PrefixLabel(labelPosition, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate positions of the various controls
        var controlsRects = GetControlsRects(position);

        // Draw controls - part 1
        // EditorGUI.LabelField(posLabelMin, min.ToString());
        EditorGUI.BeginChangeCheck();

        EditorGUI.FloatField(controlsRects[0], float.Parse(min.ToString("F2")));
        EditorGUI.MinMaxSlider(controlsRects[1], ref min, ref max, RangeAttribute.min, RangeAttribute.max);
        EditorGUI.FloatField(controlsRects[2], float.Parse(max.ToString("F2")));


        // Ensure values are in correct format (rounded and between valid ranges)
        min = Mathf.Clamp(Mathf.Round(min * 100) / 100, RangeAttribute.min, RangeAttribute.max);
        max = Mathf.Clamp(Mathf.Round(max * 100) / 100, RangeAttribute.min, RangeAttribute.max);

        // Update field on serialised object
        var fieldName = property.name;

        if (EditorGUI.EndChangeCheck())
        {
            var vector = property.vector2Value;
            vector.x = min;
            vector.y = max;
            property.vector2Value = vector;
        }
    }

    private static Rect[] GetControlsRects(Rect position)
    {
        var unitWidthSize = position.width / 10;
        var x = position.x;
        var y = position.y;
        var height = position.height;
        var offset = 10;

       // Debug.Log("unitWidthSize: " + unitWidthSize);

        var posLblMin = new Rect(x, y, unitWidthSize, height);
        var posSlider = new Rect(x + unitWidthSize + offset, y, unitWidthSize * 8 - offset * 2, height);
        var posLblMax = new Rect(x + unitWidthSize * 9, y, unitWidthSize, height);

        var rects = new Rect[3]{
          posLblMin,
          posSlider,
          posLblMax
        };

        return rects;
    }
}