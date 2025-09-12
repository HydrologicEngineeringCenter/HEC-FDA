using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.LifeLoss;
public partial class LifeSimImporterVM : BaseEditorVM
{
    private BaseViewModel _CurrentVM;
    public BaseViewModel CurrentVM
    {
        get { return _CurrentVM; }
        set { _CurrentVM = value; NotifyPropertyChanged(); }
    }

    private IndexPointsLifeLossVM _indexPointsVM;

    // this constructor is called when creating a brand new stage-LL element
    public LifeSimImporterVM(EditorActionManager actionManager) : base(actionManager)
    {
        _indexPointsVM = new IndexPointsLifeLossVM();
        RegisterChildViewModel(_indexPointsVM);
        CurrentVM = _indexPointsVM;
    }

    // this constructor is called when editing an existing stage-LL element
    public LifeSimImporterVM(ChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
    {
        StageLifeLossElement element = (StageLifeLossElement)elem;
        Name = element.Name;
        Description = element.Description;
        _indexPointsVM = new(
            element.ID,
            element.LifeSimDatabasePath,
            element.SelectedHydraulics,
            element.SelectedIndexPoints,
            element.SelectedSimulation,
            element.SelectedAlternatives,
            element.SelectedHazardTimes);
        RegisterChildViewModel(_indexPointsVM);
        CurrentVM = _indexPointsVM;
    }

    public override void Save()
    {
        FdaValidationResult vr = _indexPointsVM.ValidateForm();
        if (vr.IsValid)
        {
            string lastEditDate = DateTime.Now.ToString("G");
            int id = GetID();
            LifeSimImporterConfig config = BuildIndexPointsImporterConfig(_indexPointsVM);
            StageLifeLossElement elemToSave = new(Name, lastEditDate, Description, id, config);

            // this exists to separate the editing of the metadata and relationships
            // in stage-damage, changing just one character in the name would require every single histogram to be re-saved 
            Save(elemToSave); // base editor's save, saves the metadeta as XML
            SaveFunctionsToSQLite(_indexPointsVM.LifeLossFunctions, id); // save the curves to SQLite
        }
    }

    private LifeSimImporterConfig BuildIndexPointsImporterConfig(IndexPointsLifeLossVM indexPointsLifeLossVM)
    {
        string selectedPath = indexPointsLifeLossVM.SelectedPath;
        int hydraulicsID = indexPointsLifeLossVM.SelectedHydraulics.ID;
        int indexPointsID = indexPointsLifeLossVM.SelectedIndexPoints.ID;
        string selectedSimulation = indexPointsLifeLossVM.SelectedSimulation?.Name ?? "";
        List<string> selectedAlternatives = [];
        foreach (CheckableItem alternative in indexPointsLifeLossVM.LifeSimAlternatives)
            if (alternative.IsChecked) selectedAlternatives.Add(alternative.Name);
        List<string> selectedHazardTimes = [];
        foreach (CheckableItem hazardTime in indexPointsLifeLossVM.HazardTimes)
            if (hazardTime.IsChecked) selectedHazardTimes.Add(hazardTime.Name);

        LifeSimImporterConfig config = new()
        {
            LifeSimDatabasePath = selectedPath,
            SelectedHydraulics = hydraulicsID,
            SelectedIndexPoints = indexPointsID,
            SelectedSimulation = selectedSimulation,
            SelectedAlternatives = selectedAlternatives,
            SelectedHazardTimes = selectedHazardTimes,
        };
        return config;
    }

    private void SaveFunctionsToSQLite(List<LifeLossFunction> functions, int id)
    {
        // only save relationships if they were recomputed
        if (!_indexPointsVM.WasRecomputed)
            return;

        string projFile = Connection.Instance.ProjectFile;
        LifeLossFunctionSaver saver = new(projFile);
        LifeLossFunctionFilter filter = new()
        {
            ElementId = [id],
        };
        saver.DeleteFromSQLite(filter);
        foreach (LifeLossFunction function in functions)
        {
            function.ElementID = id;
            saver.SaveToSQLite(function);
        }
    }

    private int GetID()
    {
        if (IsCreatingNewElement)
        {
            return Saving.PersistenceFactory.GetElementManager<StageLifeLossElement>().GetNextAvailableId();
        }
        else
        {
            return OriginalElement.ID;
        }
    }
}
