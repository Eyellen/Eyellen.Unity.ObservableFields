namespace Eyellen.Unity.ObservableFields
{
    /// <summary>
    /// Interface to access read only value and methods to subscribe on changes.
    /// </summary>
    public interface IReadOnlyObservableField<T> : IObservableFieldEvents<T>
    {
        public T Value { get; }
    }
}
