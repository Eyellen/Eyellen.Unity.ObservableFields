using System;

namespace Eyellen.Unity.ObservableFields
{
    /// <summary>
    /// This interface provides methods to subscribe or unsubscribe from changes.
    /// </summary>
    public interface IObservableFieldEvents<T>
    {
        public void SubscribeOnChange(Action<ObservableField<T>.EventArgs> callback);

        public void SubscribeOnChange(Action<T, T> callback);

        public void SubscribeOnChange(Action callback);

        public void UnsubscribeOnChange(Action<ObservableField<T>.EventArgs> callback);

        public void UnsubscribeOnChange(Action<T, T> callback);

        public void UnsubscribeOnChange(Action callback);
    }
}
