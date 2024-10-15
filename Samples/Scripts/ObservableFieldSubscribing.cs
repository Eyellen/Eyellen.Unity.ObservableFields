using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Examples
{
    public class ObservableFieldSubscribing : MonoBehaviour
    {
        // You can subscribe to observable field changes in code and in inspector.
        // NOTE: To subscribe on changes in the inspector use UnityObservableField.

        private ObservableField<int> Number;

        [SerializeField]
        private UnityObservableField<string> String;

        private void Awake()
        {
            // ObservableField.
            // Subscribing to C# events.
            Number.SubscribeOnChange(OnValueChangedEventArgs);
            Number.SubscribeOnChange(OnValueChangedTTArgs);
            Number.SubscribeOnChange(OnValueChangedNoArgs);

            // UnityObservableField.
            // Subscribing to C# events.
            String.SubscribeOnChange(OnValueChangedEventArgs);
            String.SubscribeOnChange(OnValueChangedTTArgs);
            String.SubscribeOnChange(OnValueChangedNoArgs);
        }

        // Method that takes ObservableField.EventArgs as a parameter.
        // ObservableField.EventArgs contain previous and current values.
        private void OnValueChangedEventArgs<T>(ObservableField<T>.EventArgs args)
        {
            Debug.Log(
                $"Call {nameof(OnValueChangedEventArgs)}: Previous value - {args.Previous}. Current value - {args.Current}"
            );
        }

        // Method that takes previous and current value as a parameters.
        private void OnValueChangedTTArgs<T>(T previous, T current)
        {
            Debug.Log(
                $"Call {nameof(OnValueChangedTTArgs)}: Previous value - {previous}. Current value - {current}"
            );
        }

        // Method that takes no parameters.
        private void OnValueChangedNoArgs()
        {
            Debug.Log($"Call {nameof(OnValueChangedNoArgs)}");
        }
    }
}
