using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Examples
{
    public class ObservableFieldGetterSetters : MonoBehaviour
    {
        // You can add Getter/Setter like behaviour to observable fields and specify Get/Set logic.
        // Getter/Setter behaviour works fine with an inspector.
        // Getter/Setter's are passed into a field constructor as a Func<T, T>
        // where first value is input value and the second one is processed result.

        // Here we implemented setter that clamps passed value between 0 and 10.
        [SerializeField]
        private UnityObservableField<int> ObservableInt =
            new(default, value => Mathf.Clamp(value, 0, 10));

        // Here we implemented getter that simply returns the passed float number as a negative while keeping original value as positive.
        [SerializeField]
        private UnityObservableField<float> ObservableFloat = new(default, null, value => -value);
    }
}
