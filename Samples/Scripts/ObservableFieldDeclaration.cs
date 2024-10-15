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
