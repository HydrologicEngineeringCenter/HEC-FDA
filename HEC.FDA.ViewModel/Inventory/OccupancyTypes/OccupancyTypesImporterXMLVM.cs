using CommunityToolkit.Mvvm.Input;
using HEC.FDA.ViewModel.Editors;
using SciChart.Core.Extensions;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes;
public partial class OccupancyTypesImporterXMLVM : BaseEditorVM
{
    private string _path;

    public string Path
    {
        get { return _path; }
        set { _path = value; NotifyPropertyChanged(); }
    }

    public OccupancyTypesImporterXMLVM(EditorActionManager actionManager) : base(actionManager)
    {

    }

    public override void Save()
    {
        // this will only be called when creating a new element (importing)
        XElement occtypesRoot = XElement.Load(Path);
        // have to find a new valid ID because this XML file could have come from any FDA study, IDs are scoped to studies
        int id = Saving.PersistenceFactory.GetElementManager<OccupancyTypesElement>().GetNextAvailableId();

        XElement header = occtypesRoot.Element("Header");
        if (header == null)
            throw new System.FormatException();

        header.SetAttributeValue("ID", id);
        // prepend the base name the user gives to the original occtypes name
        string originalName = (string)header.Attribute("Name") ?? string.Empty;
        header.SetAttributeValue("Name", $"{Name} {originalName}");
        OccupancyTypesElement occtypes = new(occtypesRoot, id);
        Save(occtypes);
    }

    [RelayCommand]
    public void Import()
    {
        if (Path.IsNullOrEmpty())
            return;

        Save();
    }
}
