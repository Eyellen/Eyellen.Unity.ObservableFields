#if UNITY_EDITOR && !OBSERVABLE_FIELDS_DISABLE_EDITOR
namespace Eyellen.Unity.ObservableFields
{
    public interface IUndoRedoStack
    {
        void Push();
    }
}
#endif
