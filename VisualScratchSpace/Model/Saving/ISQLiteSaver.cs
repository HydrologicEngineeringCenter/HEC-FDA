using Amazon.Runtime;

namespace VisualScratchSpace.Model.Saving;
public  interface ISQLiteSaver<T> : IDisposable
{
    public void SaveToSQLite(T item);

    public List<T> ReadFromSQLite(SQLiteFilter filter, bool selectAll = false);
}
