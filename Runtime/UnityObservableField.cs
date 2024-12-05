using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eyellen.Unity.ObservableFields
{
    [Serializable]
    public class UnityObservableField<T> : ObservableField<T>
    {
#if UNITY_EDITOR
        [SerializeField]
        private T m_SerializedValue;

        [SerializeField]
        [HideInInspector]
        private bool m_ShowUnityEvents;
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
#if UNITY_EDITOR
            m_SerializedValue = value;
#endif
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_EDITOR
            SubscribeOnChange(OnValueChanged);
#endif
            SubscribeOnChange(InvokeUnityEvents);
        }

#if UNITY_EDITOR
        private void OnValueChanged(EventArgs args)
        {
            m_SerializedValue = args.Current;
        }
#endif

        private void InvokeUnityEvents(EventArgs args)
        {
            if (!m_UseUnityEvents)
                return;

            m_OnValueChangedEventArgs.Invoke(args);
            m_OnValueChangedTTArgs.Invoke(args.Previous, args.Current);
            m_OnValueChangedCurrentArg.Invoke(args.Current);
            m_OnValueChangedNoArgs.Invoke();
        }
    }
}
