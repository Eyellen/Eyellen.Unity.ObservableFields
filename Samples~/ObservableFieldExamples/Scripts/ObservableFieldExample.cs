using UnityEngine;

namespace Eyellen.Unity.ObservableFields.Examples
{
    public class ObservableFieldExample : MonoBehaviour
    {
        [SerializeField]
        private UnityObservableField<bool> IsCubeActive = new();

        [SerializeField]
        private UnityObservableField<string> Label = new();

        [SerializeField]
        private UnityObservableField<Color> Color = new();

        private void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(500));
            {
                IsCubeActive.Value = GUILayout.Toggle(IsCubeActive, "Toggle");
                Label.Value = GUILayout.TextField(Label);
            }
            GUILayout.EndVertical();
        }
    }
}
