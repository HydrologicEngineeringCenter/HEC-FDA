using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class StageLifeLossElement : ChildElement
{
    private const string CONFIG = "Config";
    private const string LIFESIM_DATABASE_PATH = "LifeSimDatabaseFileName";
    private const string SELECTED_HYDRAULICS = "SelectedHydraulics";
    private const string SELECTED_INDEX_POINTS = "SelectedIndexPoints";
    private const string SELECTED_SIMULATION = "SelectedSimulation";
    private const string SELECTED_ALTERNATIVES = "SelectedAlternatives";
    private const string SELECTED_HAZARD_TIMES = "SelectedHazardTimes";
    private const string ALTERNATIVE = "Alternative";
    private const string HAZARD_TIME = "HazardTime";

    public string LifeSimDatabaseFileName { get; }
    public int SelectedHydraulics { get; }
    public int SelectedIndexPoints { get; }
    public string SelectedSimulation { get; }
    public List<string> SelectedAlternatives { get; } = [];
    public List<string> SelectedHazardTimes { get; } = [];

    // gets called when saving the element to sqlite (either new or existing)
    public StageLifeLossElement(string name, string lastEditDate, string description, int id, LifeSimImporterConfig config)
        : base(name, lastEditDate, description, id)
    {
        LifeSimDatabaseFileName = config.LifeSimDatabaseFileName;
        SelectedHydraulics = config.SelectedHydraulics;
        SelectedIndexPoints = config.SelectedIndexPoints;
        SelectedSimulation = config.SelectedSimulation;
        SelectedAlternatives = config.SelectedAlternatives;
        SelectedHazardTimes = config.SelectedHazardTimes;

        AddDefaultActions(EditStageLifeLossCurves, StringConstants.EDIT_STAGE_LIFE_LOSS_FUNCTION);
    }

    // gets called when loading the element from sqlite. Has no references because we're getting here through reflection. 
    public StageLifeLossElement(XElement elementXML, int id) : base(elementXML, id)
    {
        XElement config = elementXML.Element(CONFIG);
        LifeSimDatabaseFileName = config.Element(LIFESIM_DATABASE_PATH).Value;
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
        config.Add(new XElement(LIFESIM_DATABASE_PATH, LifeSimDatabaseFileName ?? ""));
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

    public List<LifeLossFunction> LoadStageLifeLossRelationships()
    {
        string projFile = Connection.Instance.ProjectFile;
        List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        Dictionary<string, int> IANameToID = impactAreaElements[0].GetNameToIDPairs();
        LifeLossFunctionSaver saver = new(projFile, IANameToID);
        LifeLossFunctionFilter filter = new() { Element_ID = [ID] };
        return saver.ReadFromSQLite(filter);
    }

    /// <summary>
    /// Retrieves a collection of stage-life loss relationships represented as uncertain paired data.
    /// </summary>
    /// <returns>A list of <see cref="UncertainPairedData"/> objects, each representing a stage-life loss relationship. The list
    /// will be empty if no relationships are available.</returns>
    public List<UncertainPairedData> StageLifeLossRelationshipsAsUPD()
    {
        List<LifeLossFunction> lifeLossFunctions = LoadStageLifeLossRelationships();
        return lifeLossFunctions.Select((f) => f.Data).ToList();
    }

    /// <summary>
    /// Retrieves stage-life loss relationships filtered by impact area ID.
    /// </summary>
    /// <param name="impactAreaID">The impact area ID to filter by.</param>
    /// <returns>A list of <see cref="UncertainPairedData"/> objects for the specified impact area.</returns>
    public List<UncertainPairedData> StageLifeLossRelationshipsAsUPD(int impactAreaID)
    {
        List<LifeLossFunction> lifeLossFunctions = LoadStageLifeLossRelationships();
        return lifeLossFunctions
            .Where(f => f.Data.CurveMetaData.ImpactAreaID == impactAreaID)
            .Select(f => f.Data)
            .ToList();
    }
}

