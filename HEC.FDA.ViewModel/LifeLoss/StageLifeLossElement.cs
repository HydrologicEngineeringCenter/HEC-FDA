using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
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

        AddDefaultActions(EditStageLifeLossCurves);
    }

    // gets called when loading the element from sqlite
    public StageLifeLossElement(XElement elementXML, int id) : base(elementXML, id)
    {
        SelectedHydraulics = Convert.ToInt32(elementXML.Attribute(SELECTED_HYDRAULICS).Value);
        SelectedIndexPoints = Convert.ToInt32(elementXML.Attribute(SELECTED_INDEX_POINTS).Value);
        AddDefaultActions(EditStageLifeLossCurves);
    }


    public override XElement ToXML()
    {
        XElement stageLifeLossElem = new(StringConstants.ELEMENT_XML_TAG);
        stageLifeLossElem.Add(CreateHeaderElement());
        stageLifeLossElem.SetAttributeValue(SELECTED_HYDRAULICS, SelectedHydraulics);
        stageLifeLossElem.SetAttributeValue(SELECTED_INDEX_POINTS, SelectedIndexPoints);
        return stageLifeLossElem;
    }

    private void EditStageLifeLossCurves(object arg1, EventArgs arg2)
    {
        EditorActionManager actionManager = new EditorActionManager().WithSiblingRules(this);

        LifeSimImporterVM vm = new(this, actionManager);

        string title = "Edit " + vm.Name;
        DynamicTabVM tab = new(title, vm, "EditStageLifeLossElement" + Name);
        Navigate(tab, false, true);
    }
}

