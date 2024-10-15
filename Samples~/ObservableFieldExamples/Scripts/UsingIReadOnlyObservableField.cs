using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Examples
{
    public class UsingIReadOnlyObservableField : MonoBehaviour
    {
        // You can use IReadOnlyObservableField if you want to allow readonly value on you observable field
        // but want to protect it from changes from outside the class.

        // Private observable field.
        // Can't be accessed outside the class.
        [SerializeField]
        private UnityObservableField<float> _float = new();

        // Public IReadOnlyObservableField.
        // Can be accessed outside the class.
        // Provides only readonly Value getter and methods to subscribe to change event.
        public IReadOnlyObservableField<float> Float => _float;
    }
}
