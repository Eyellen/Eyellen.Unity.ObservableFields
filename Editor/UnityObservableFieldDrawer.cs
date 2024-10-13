using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnityObservableField<>))]
public class UnityObservableFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool hasBeenChanged = false;

        EditorGUI.BeginProperty(position, label, property);
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(
                position,
                property.FindPropertyRelative("m_SerializedValue"),
                label
            );
            hasBeenChanged = EditorGUI.EndChangeCheck();
        }
        EditorGUI.EndProperty();

        if (property.serializedObject.hasModifiedProperties)
            property.serializedObject.ApplyModifiedProperties();

        if (hasBeenChanged)
        {
            BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            UnityEngine.Object targetObject = property.serializedObject.targetObject;

            object propertyValue = targetObject
                .GetType()
                .GetField(property.name, flags)
                .GetValue(targetObject);

            Type propertyType = propertyValue.GetType();

            object value = propertyType
                .GetField("m_SerializedValue", flags)
                .GetValue(propertyValue);

            MethodInfo setMethod = propertyType.BaseType.GetProperty("Value", flags).GetSetMethod();
            setMethod.Invoke(propertyValue, new[] { value });

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
