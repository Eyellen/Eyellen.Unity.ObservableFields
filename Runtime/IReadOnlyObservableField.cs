using System;

namespace Eyellen.Unity.ObservableFields
{
    /// <summary>
    /// Interface to access read only value and methods to subscribe on changes.
    /// </summary>
    public interface IReadOnlyObservableField<T>
    {
        public T Value { get; }

        public void SubscribeOnChange(Action<ObservableField<T>.EventArgs> callback);

        public void SubscribeOnChange(Action<T, T> callback);

        public void SubscribeOnChange(Action callback);

        public void UnsubscribeOnChange(Action<ObservableField<T>.EventArgs> callback);

        public void UnsubscribeOnChange(Action<T, T> callback);

        public void UnsubscribeOnChange(Action callback);
    }
}
