using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Editor
{
    [CustomPropertyDrawer(typeof(UnityObservableField<>))]
    public class UnityObservableFieldDrawer : PropertyDrawer
    {
        private const string m_SerializedValuePath = "m_SerializedValue";
        private const string m_UseUnityEventsPath = "m_UseUnityEvents";
        private const string m_OnValueChangedEventArgsPath = "m_OnValueChangedEventArgs";
        private const string m_OnValueChangeTTArgsPath = "m_OnValueChangedTTArgs";
        private const string m_OnValueChangeNoArgsPath = "m_OnValueChangedNoArgs";

        private bool m_ShowUnityEvents;

        private GUIContent m_EventLabel = new GUIContent("On Value Changed");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool hasBeenChanged = false;

            EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(
                    property.FindPropertyRelative(m_SerializedValuePath),
                    label
                );
                hasBeenChanged = EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property.FindPropertyRelative(m_UseUnityEventsPath));
                bool useUnityEvents = property.FindPropertyRelative(m_UseUnityEventsPath).boolValue;
                if (useUnityEvents)
                {
                    m_ShowUnityEvents = EditorGUILayout.Foldout(m_ShowUnityEvents, "Events");

                    if (m_ShowUnityEvents)
                    {
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(m_OnValueChangedEventArgsPath),
                            m_EventLabel
                        );
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(m_OnValueChangeTTArgsPath),
                            m_EventLabel
                        );
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(m_OnValueChangeNoArgsPath),
                            m_EventLabel
                        );
                    }
                }
                EditorGUI.indentLevel--;
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
                    .GetField(m_SerializedValuePath, flags)
                    .GetValue(propertyValue);

                MethodInfo setMethod = propertyType
                    .BaseType.GetProperty("Value", flags)
                    .GetSetMethod();
                setMethod.Invoke(propertyValue, new[] { value });

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
