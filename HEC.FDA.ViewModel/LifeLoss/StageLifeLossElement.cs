using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class StageLifeLossElement : ChildElement
{
    private const string LIFESIM_DATABASE_PATH = "LifeSimDatabasePath";
    private const string SELECTED_HYDRAULICS = "SelectedHydraulics";
    private const string SELECTED_INDEX_POINTS = "SelectedIndexPoints";
    private const string SELECTED_SIMULATION = "SelectedSimulation";
    private const string SELECTED_ALTERNATIVES = "SelectedAlternatives";
    private const string SELECTED_HAZARD_TIMES = "SelectedHazardTimes";

    public string LifeSimDatabasePath { get; }
    public int SelectedHydraulics { get; }
    public int SelectedIndexPoints { get; }
    public string SelectedSimulation { get; }
    public List<string> SelectedAlternatives { get; }
    public List<string> SelectedHazardTimes { get; }

    // gets called when saving the element to sqlite (either new or existing)
    public StageLifeLossElement(string name, string lastEditDate, string description, int id, LifeSimImporterConfig config)
        : base(name, lastEditDate, description, id)
    {
        LifeSimDatabasePath = config.LifeSimDatabasePath;
        SelectedHydraulics = config.SelectedHydraulics;
        SelectedIndexPoints = config.SelectedIndexPoints;
        SelectedSimulation = config.SelectedSimulation;
        SelectedAlternatives = config.SelectedAlternatives;
        SelectedHazardTimes = config.SelectedHazardTimes;

        AddDefaultActions(EditStageLifeLossCurves, StringConstants.EDIT_STAGE_LIFE_LOSS_FUNCTION);
    }

    // gets called when loading the element from sqlite
    public StageLifeLossElement(XElement elementXML, int id) : base(elementXML, id)
    {
        LifeSimDatabasePath = elementXML.Attribute(LIFESIM_DATABASE_PATH).Value;
        SelectedHydraulics = Convert.ToInt32(elementXML.Attribute(SELECTED_HYDRAULICS).Value);
        SelectedIndexPoints = Convert.ToInt32(elementXML.Attribute(SELECTED_INDEX_POINTS).Value);
        SelectedSimulation = elementXML.Attribute(SELECTED_SIMULATION).Value;
        string rawAlternatives = elementXML.Attribute(SELECTED_ALTERNATIVES).Value;
        SelectedAlternatives = string.IsNullOrEmpty(rawAlternatives) ? [] : rawAlternatives.Split(',').ToList();
        string rawHazardTimes = elementXML.Attribute(SELECTED_HAZARD_TIMES).Value;
        SelectedHazardTimes = string.IsNullOrEmpty(rawHazardTimes) ? [] : rawHazardTimes.Split(',').ToList();
        AddDefaultActions(EditStageLifeLossCurves, StringConstants.EDIT_STAGE_LIFE_LOSS_FUNCTION);
    }


    public override XElement ToXML()
    {
        XElement stageLifeLossElem = new(StringConstants.ELEMENT_XML_TAG);
        stageLifeLossElem.Add(CreateHeaderElement());
        stageLifeLossElem.SetAttributeValue(LIFESIM_DATABASE_PATH, LifeSimDatabasePath ?? "");
        stageLifeLossElem.SetAttributeValue(SELECTED_HYDRAULICS, SelectedHydraulics);
        stageLifeLossElem.SetAttributeValue(SELECTED_INDEX_POINTS, SelectedIndexPoints);
        stageLifeLossElem.SetAttributeValue(SELECTED_SIMULATION, SelectedSimulation ?? "");
        stageLifeLossElem.SetAttributeValue(SELECTED_ALTERNATIVES, string.Join(",", SelectedAlternatives ?? []));
        stageLifeLossElem.SetAttributeValue(SELECTED_HAZARD_TIMES, string.Join(",", SelectedHazardTimes ?? []));

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

