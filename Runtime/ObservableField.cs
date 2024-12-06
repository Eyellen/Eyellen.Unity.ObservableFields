using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eyellen.Unity.ObservableFields
{
    /// <summary>
    /// Class that contains a value and an event that fires when the value changes.
    /// </summary>
    [Serializable]
    public class ObservableField<T> : IReadOnlyObservableField<T>
    {
        public readonly struct EventArgs
        {
            public readonly T Previous;
            public readonly T Current;

            public EventArgs(T previous, T current)
            {
                Previous = previous;
                Current = current;
            }
        }

        [SerializeField]
        private T m_Value;

        public T Value
        {
            get => m_GetterFunc.Invoke(m_Value);
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, m_Value))
                    return;

                T previous = m_Value;
                m_Value = m_SetterFunc.Invoke(value);
                InvokeEvents(previous, m_Value);
            }
        }

        private Func<T, T> m_SetterFunc = value => value;

        private Func<T, T> m_GetterFunc = value => value;

        private Action<EventArgs> m_OnChangedEventArgs;

        private Action<T, T> m_OnChangedTTArgs;

        private Action m_OnChangedNoArgs;

        /// <summary>
        /// If you want to assign an initial value you can pass it to a constructor.
        /// If you want to make a property-like behaviour and add getter/setter logic
        /// you can assign setterFunc and getterFunc.
        /// </summary>
        /// <param name="value">Initial value</param>
        /// <param name="setterFunc">SetterFunc that processes a value before assigning it</param>
        /// <param name="getterFunc">GetterFunc that processes a value before returning it</param>
        public ObservableField(
            T value = default,
            Func<T, T> setterFunc = null,
            Func<T, T> getterFunc = null
        )
        {
            m_Value = value;

            if (setterFunc != null)
                m_SetterFunc = setterFunc;

            if (getterFunc != null)
                m_GetterFunc = getterFunc;
        }

        public static implicit operator T(ObservableField<T> field)
        {
            return field.Value;
        }

        private void InvokeEvents(T previous, T current)
        {
            m_OnChangedEventArgs?.Invoke(new EventArgs(previous, current));
            m_OnChangedTTArgs?.Invoke(previous, current);
            m_OnChangedNoArgs?.Invoke();
        }

        public void SubscribeOnChange(Action<EventArgs> callback)
        {
            m_OnChangedEventArgs += callback;
        }

        public void SubscribeOnChange(Action<T, T> callback)
        {
            m_OnChangedTTArgs += callback;
        }

        public void SubscribeOnChange(Action callback)
        {
            m_OnChangedNoArgs += callback;
        }

        public void UnsubscribeOnChange(Action<EventArgs> callback)
        {
            m_OnChangedEventArgs -= callback;
        }

        public void UnsubscribeOnChange(Action<T, T> callback)
        {
            m_OnChangedTTArgs -= callback;
        }

        public void UnsubscribeOnChange(Action callback)
        {
            m_OnChangedNoArgs -= callback;
        }
    }
}
