using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeLossOwnerElement : ParentElement
{
    public LifeLossOwnerElement(): base()
    {
        Name = "Aggregated Stage-Life Loss Functions";
        CustomTreeViewHeader = new CustomHeaderVM(Name);
        IsBold = false;
        NamedAction add = new()
        {
            Header = "Import LifeSim Database...",
            Action = AddNew
        };
        List<NamedAction> localActions = new()
        {
            add
        };
        Actions = localActions; 

        StudyCache.StageLifeLossAdded += AddLifeLossElement;
        StudyCache.StageLifeLossRemoved += RemoveLifeLossElement;
        StudyCache.StageLifeLossUpdated += UpdateLifeLossElement;
    }

    private void UpdateLifeLossElement(object sender, ElementUpdatedEventArgs e)
    {
        UpdateElement(e.NewElement);
    }
    private void RemoveLifeLossElement(object sender, ElementAddedEventArgs e)
    {
        RemoveElement(e.Element);
    }
    private void AddLifeLossElement(object sender, ElementAddedEventArgs e)
    {
        AddElement(e.Element);
    }

    private void AddNew(object arg1, EventArgs arg2)
    {
        EditorActionManager actionManager = new EditorActionManager().WithSiblingRules(this);
        LifeSimImporterVM vm = new(actionManager);
        DynamicTabVM tab = new("Import LifeSim", vm, "Import LifeSim");
        Navigate(tab, false, true);
    }
}
