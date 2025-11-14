using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class StageLifeLossElement : ChildElement
{
    private const string CONFIG = "Config";
    private const string LIFESIM_DATABASE_PATH = "LifeSimDatabasePath";
    private const string SELECTED_HYDRAULICS = "SelectedHydraulics";
    private const string SELECTED_INDEX_POINTS = "SelectedIndexPoints";
    private const string SELECTED_SIMULATION = "SelectedSimulation";
    private const string SELECTED_ALTERNATIVES = "SelectedAlternatives";
    private const string SELECTED_HAZARD_TIMES = "SelectedHazardTimes";
    private const string ALTERNATIVE = "Alternative";
    private const string HAZARD_TIME = "HazardTime";

    public string LifeSimDatabasePath { get; }
    public int SelectedHydraulics { get; }
    public int SelectedIndexPoints { get; }
    public string SelectedSimulation { get; }
    public List<string> SelectedAlternatives { get; } = [];
    public List<string> SelectedHazardTimes { get; } = [];

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
        XElement config = elementXML.Element(CONFIG);
        LifeSimDatabasePath = config.Element(LIFESIM_DATABASE_PATH).Value;
        SelectedHydraulics = Convert.ToInt32(config.Element(SELECTED_HYDRAULICS).Value);
        SelectedIndexPoints = Convert.ToInt32(config.Element(SELECTED_INDEX_POINTS).Value);
        SelectedSimulation = config.Element(SELECTED_SIMULATION).Value;
        XElement alternativesParent = config.Element(SELECTED_ALTERNATIVES);
        IEnumerable<XElement> alternatives = alternativesParent.Elements(ALTERNATIVE);
        foreach (XElement alternative in alternatives)
            SelectedAlternatives.Add(alternative.Value);
        XElement hazardTimesParent = config.Element(SELECTED_HAZARD_TIMES);
        IEnumerable<XElement> hazardTimes = hazardTimesParent.Elements(HAZARD_TIME);
        foreach (XElement hazardTime in hazardTimes)
            SelectedHazardTimes.Add(hazardTime.Value);

        AddDefaultActions(EditStageLifeLossCurves, StringConstants.EDIT_STAGE_LIFE_LOSS_FUNCTION);
    }


    public override XElement ToXML()
    {
        XElement stageLifeLossElem = new(StringConstants.ELEMENT_XML_TAG);
        stageLifeLossElem.Add(CreateHeaderElement());
        XElement config = new(CONFIG);
        config.Add(new XElement(LIFESIM_DATABASE_PATH, LifeSimDatabasePath ?? ""));
        config.Add(new XElement(SELECTED_HYDRAULICS, SelectedHydraulics));
        config.Add(new XElement(SELECTED_INDEX_POINTS, SelectedIndexPoints));
        config.Add(new XElement(SELECTED_SIMULATION, SelectedSimulation ?? ""));
        XElement alternatives = new(SELECTED_ALTERNATIVES, SelectedAlternatives?.Select(a => new XElement(ALTERNATIVE, a)));
        config.Add(alternatives);
        XElement hazardTimes = new(SELECTED_HAZARD_TIMES, SelectedHazardTimes?.Select(h => new XElement(HAZARD_TIME, h)));
        config.Add(hazardTimes);
        stageLifeLossElem.Add(config);

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

