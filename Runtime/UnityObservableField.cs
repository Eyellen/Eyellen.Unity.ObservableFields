using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Eyellen.Unity.ObservableFields
{
    [Serializable]
    public class UnityObservableField<T> : ObservableField<T>, IDisposable
#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
            , IUndoRedoStack
    {
        [SerializeField]
        private T m_SerializedValue;

        [SerializeField]
        [HideInInspector]
        private bool m_ShowUnityEvents;

        private Stack<int> m_UndoGroupStack = new();
#else
    {
#endif
        [SerializeField]
        [Tooltip("Check this if you want the ObservableField to call Unity Events")]
        private bool m_UseUnityEvents;

        [SerializeField]
        private UnityEvent<EventArgs> m_OnValueChangedEventArgs;

        [SerializeField]
        private UnityEvent<T, T> m_OnValueChangedTTArgs;

        [SerializeField]
        private UnityEvent<T> m_OnValueChangedCurrentArg;

        [SerializeField]
        private UnityEvent m_OnValueChangedNoArgs;

        public UnityObservableField()
            : base()
        {
            Initialize();
        }

        public UnityObservableField(
            T value = default,
            Func<T, T> setterFunc = null,
            Func<T, T> getterFunc = null
        )
            : base(value, setterFunc, getterFunc)
        {
#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
            m_SerializedValue = value;
#endif
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
            SubscribeOnChange(OnValueChanged);
#endif
            SubscribeOnChange(InvokeUnityEvents);
        }

        public void Dispose()
        {
#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
            Undo.undoRedoEvent -= OnUndoRedo;
            m_UndoGroupStack.Clear();
#endif
        }

        private void InvokeUnityEvents(EventArgs args)
        {
            if (!m_UseUnityEvents)
                return;

            m_OnValueChangedEventArgs.Invoke(args);
            m_OnValueChangedTTArgs.Invoke(args.Previous, args.Current);
            m_OnValueChangedCurrentArg.Invoke(args.Current);
            m_OnValueChangedNoArgs.Invoke();
        }

#if UNITY_EDITOR && !OBSERVABLE_FIELDS_EDITOR_DISABLE
        private void OnValueChanged(EventArgs args)
        {
            m_SerializedValue = args.Current;
        }

        private void OnUndoRedo(in UndoRedoInfo info)
        {
            if (m_UndoGroupStack.Count <= 0)
            {
                Undo.undoRedoEvent -= OnUndoRedo;
                return;
            }

            if (info.undoGroup != m_UndoGroupStack.Peek())
                return;

            m_UndoGroupStack.Pop();

            Value = m_SerializedValue;
        }

        void IUndoRedoStack.Push()
        {
            Undo.undoRedoEvent -= OnUndoRedo;
            Undo.undoRedoEvent += OnUndoRedo;

            int group = Undo.GetCurrentGroup();
            m_UndoGroupStack.Push(group);
        }
#endif
    }
}
