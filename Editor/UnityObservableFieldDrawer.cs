using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Editor
{
    [CustomPropertyDrawer(typeof(UnityObservableField<>))]
    public class UnityObservableFieldDrawer : PropertyDrawer
    {
        private const string k_SerializedValuePath = "m_SerializedValue";
        private const string k_UseUnityEventsPath = "m_UseUnityEvents";
        private const string k_OnValueChangedEventArgsPath = "m_OnValueChangedEventArgs";
        private const string k_OnValueChangeTTArgsPath = "m_OnValueChangedTTArgs";
        private const string k_OnValueChangedCurrentArgPath = "m_OnValueChangedCurrentArg";
        private const string k_OnValueChangeNoArgsPath = "m_OnValueChangedNoArgs";
        private const string k_ShowUnityEventsPath = "m_ShowUnityEvents";

        private static GUIContent s_EventLabel = new GUIContent("On Value Changed");

        private static Regex s_RegexArrayElement = new Regex(
            @"(.*)\[(\d*)\]",
            RegexOptions.Compiled
        );

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool valueHasBeenChanged = false;

            SerializedProperty valueProperty = property.FindPropertyRelative(k_SerializedValuePath);
            SerializedProperty useUnityEventsProperty = property.FindPropertyRelative(
                k_UseUnityEventsPath
            );
            SerializedProperty showUnityEvents = property.FindPropertyRelative(
                k_ShowUnityEventsPath
            );

            if (valueProperty == null)
                return;

            label = EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(valueProperty, label);
                valueHasBeenChanged = EditorGUI.EndChangeCheck();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(useUnityEventsProperty);
                bool useUnityEvents = useUnityEventsProperty.boolValue;
                if (useUnityEvents)
                {
                    showUnityEvents.boolValue = EditorGUILayout.Foldout(
                        showUnityEvents.boolValue,
                        "Events"
                    );

                    if (showUnityEvents.boolValue)
                    {
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(k_OnValueChangedEventArgsPath),
                            s_EventLabel
                        );
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(k_OnValueChangeTTArgsPath),
                            s_EventLabel
                        );
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(k_OnValueChangedCurrentArgPath),
                            s_EventLabel
                        );
                        EditorGUILayout.PropertyField(
                            property.FindPropertyRelative(k_OnValueChangeNoArgsPath),
                            s_EventLabel
                        );
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();

            if (property.serializedObject.hasModifiedProperties)
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (valueHasBeenChanged)
                AssignNewValue(property);
        }

        private static void AssignNewValue(SerializedProperty property)
        {
            object target = GetPropertyTargetObject(property);

            Type targetType = target.GetType();

            BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            object newValue = targetType.GetField(k_SerializedValuePath, flags).GetValue(target);

            MethodInfo setMethod = targetType.BaseType.GetProperty("Value", flags).GetSetMethod();
            setMethod.Invoke(target, new[] { newValue });

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static object GetPropertyTargetObject(SerializedProperty property)
        {
            BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            object component = property.serializedObject.targetObject;

            string propertyPath = property.propertyPath;

            string[] fields = propertyPath.Replace(".Array.data[", "[").Split(".");

            object target = component;
            foreach (string field in fields)
            {
                Type targetType = target.GetType();

                Match match = s_RegexArrayElement.Match(field);
                bool isArray = match.Success;
                if (isArray)
                {
                    string fieldName = match.Groups[1].Value;
                    int index = int.Parse(match.Groups[2].Value);

                    IEnumerable enumerable = (IEnumerable)GetValue(target, fieldName, flags);
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    for (int i = 0; i <= index; i++)
                        enumerator.MoveNext();
                    target = enumerator.Current;
                }
                else
                {
                    target = GetValue(target, field, flags);
                }
            }

            return target;
        }

        private static object GetValue(object obj, string name, BindingFlags bindingFlags = default)
        {
            Type type = obj.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(name, bindingFlags);
                if (field != null)
                    return field.GetValue(obj);

                PropertyInfo property = type.GetProperty(name, bindingFlags);
                if (property != null)
                    return property.GetValue(obj);

                type = type.BaseType;
            }

            return null;
        }
    }
}
