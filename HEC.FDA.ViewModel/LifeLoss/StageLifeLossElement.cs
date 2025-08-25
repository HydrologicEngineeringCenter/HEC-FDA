using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class StageLifeLossElement : ChildElement
{
    private const string SELECTED_HYDRAULICS = "SelectedHydraulics";
    private const string SELECTED_INDEX_POINTS = "SelectedIndexPoints";

    public int SelectedHydraulics { get; }
    public int SelectedIndexPoints { get; }

    // gets called when saving the element to sqlite (either new or existing)
    public StageLifeLossElement(string name, string lastEditDate, string description, int id, int selectedHydraulics, int selectedIndexPoints)
        : base(name, lastEditDate, description, id)
    {
        SelectedHydraulics = selectedHydraulics;
        SelectedIndexPoints = selectedIndexPoints;

        AddDefaultActions();
    }

    // gets called when loading the element from sqlite
    public StageLifeLossElement(XElement elementXML, int id) : base(elementXML, id)
    {
        AddDefaultActions();
    }


    public override XElement ToXML()
    {
        XElement stageLifeLossElem = new(StringConstants.ELEMENT_XML_TAG);
        stageLifeLossElem.Add(CreateHeaderElement());
        stageLifeLossElem.SetAttributeValue(SELECTED_HYDRAULICS, SelectedHydraulics);
        stageLifeLossElem.SetAttributeValue(SELECTED_INDEX_POINTS, SelectedIndexPoints);
        return stageLifeLossElem;
    }
}

