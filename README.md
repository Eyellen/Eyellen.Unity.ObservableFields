# Unity Observable Fields

Simple observable fields implementation for Unity that works with an inspector.

# Installation

## Install via UPM (using Git URL)
Open Unity > Window > Package Manager > + > Add package from git URL:
```
https://github.com/Eyellen/Eyellen.Unity.ObservableFields.git
```
Paste and press "Add"

# Summary

`ObservableField` - Does not support inspector and uses only pure C# features.</br>
`UnityObservableField` - Supports inspector, uses C# and Unity events. You can assign Unity events in the inspector.</br>
`IReadOnlyObservableField` - Read only interface for observable fields. Allows to get read only value and subscribe to changes.</br>
`IObservableFieldEvents` - Interface that provides methods to subscribe or unsubscribe from changes.</br>

# Important Notes

- You must call constructor for observable fields, otherwise it will give a `NullReferenceException`.
- If you want Unity events to be called in edit mode make sure to set `UnityEventCallState` to `EditorAndRuntime` in the inspector, it's next to each method reference in the unity event property. If you want C# events to be called in edit mode make sure to add `ExecuteAlwaysAttribute` to your class.
- If you pass your custom type to `UnityObservableField` and want it to appear in the inspector make sure to add `SerializableAttribute` to your type.
- Remember that reference types are reference types, `ObservableField` will not fire events when reference type's members will be changed, it will fire event only if the reference itself is changed. If you need to fire event when complex object is changed prefer using structs.

# Usage

## Declaring observable fields:
```CSharp
using System;
using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Examples
{
    public class ObservableFieldDeclaration : MonoBehaviour
    {
        // ObservableField does not support inspector. If you want it to show up in the inspector, use UnityObservableField.
        // Does not appear in the inspector even though it has a SerializeField attribute.
        // Uses CSharp events.
        [SerializeField]
        private ObservableField<bool> ObservableBool = new();

        // Displayed in the inspector.
        // Uses CSharp events and Unity events. Unity events can be assigned in the inspector.
        [SerializeField]
        private UnityObservableField<bool> UnityObservableBool = new();

        // Displayed in the inspector since a passed type have Serializable attribute.
        [SerializeField]
        private UnityObservableField<SerializedType> SerializableType = new();

        // Does not appear in the inspector because a type passed to a field is not serializable.
        [SerializeField]
        private UnityObservableField<NonSerializedType> NonSerializableType = new();
    }

    public class NonSerializedType
    {
        public string Name;
        public string Description;
    }

    [Serializable]
    public class SerializedType
    {
        public string Name;
        public string Description;
    }
}
```

Declared `UnityObservableField`s will look like this in the inspector:</br>
![Pasted image 20241016005148](https://github.com/user-attachments/assets/216b5910-afd9-4b60-9ca3-f877239cf100)

## Subscribing to observable field changes:
```CSharp
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
```

Inspector:</br>
`Use Unity Events` - indicates if Unity events will be called. If has false value Unity events will not be called even if they were assigned.</br>
![Pasted image 20241016005315](https://github.com/user-attachments/assets/5fd2922f-0cd5-42f9-a823-d96b1b5cccf2)

If you want to use Unity events, check `Use Unity Events` and `Events` foldout will pop up:</br>
![Pasted image 20241016005415](https://github.com/user-attachments/assets/c3a774ec-3257-46db-8ba8-154547130b1b)

Click on `Events` foldout and there are 4 Unity events:
1. Passes `ObservableField.EventArgs` as a parameter
2. Passes `Previous` and `Current` values as a parameters
3. Passes `Current` value as a parameter
4. Passes no parameters</br>
![Pasted image 20241016005538](https://github.com/user-attachments/assets/064b78e7-9df6-44fb-8439-c9c672127f1f)

## Using IReadOnlyObservableField
```CSharp
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
```

## Implementing Getter/Setter's
```CSharp
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
```

# License

MIT
