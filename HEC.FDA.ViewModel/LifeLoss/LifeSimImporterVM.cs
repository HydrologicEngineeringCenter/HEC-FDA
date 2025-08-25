using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;

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

    // this constructor is for creating a brand new stage-LL element
    public LifeSimImporterVM(EditorActionManager actionManager) : base(actionManager)
    {
        _indexPointsVM = new IndexPointsLifeLossVM();
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
            int hydraulicsID = _indexPointsVM.SelectedHydraulics.ID;
            int indexPointsID = _indexPointsVM.SelectedIndexPoints.ID;
            StageLifeLossElement elemToSave = new(Name, lastEditDate, Description, id, hydraulicsID, indexPointsID);
            Save(elemToSave); // base editor's save
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
