
namespace Base.Interfaces
{
    public interface IUndoRedo: ITrackChanges
    {
        void Undo();
        void Redo();
    }
}
