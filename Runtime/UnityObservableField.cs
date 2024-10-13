using System;
using UnityEngine;

[Serializable]
public class UnityObservableField<T> : ObservableField<T>
{
#if UNITY_EDITOR
    [SerializeField]
    private T m_SerializedValue;
#endif

    public UnityObservableField(
        T value = default,
        Func<T, T> setterFunc = null,
        Func<T, T> getterFunc = null
    )
        : base(value, setterFunc, getterFunc)
    {
#if UNITY_EDITOR
        m_SerializedValue = value;
        SubscribeOnChange(OnValueChanged);
#endif
    }

#if UNITY_EDITOR
    private void OnValueChanged(EventArgs args)
    {
        m_SerializedValue = args.Current;
    }
#endif
}
