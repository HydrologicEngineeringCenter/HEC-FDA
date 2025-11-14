using HEC.FDA.ViewModel.Editors;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes;
public partial class OccupancyTypesImporterSQLiteVM : BaseEditorVM
{
    private string _path;

    public string Path
    {
        get { return _path; }
        set { _path = value; NotifyPropertyChanged(); }
    }

    public OccupancyTypesImporterSQLiteVM(EditorActionManager actionManager) : base(actionManager)
    {

    }

    public override void Save()
    {
        throw new System.NotImplementedException();
    }
}
