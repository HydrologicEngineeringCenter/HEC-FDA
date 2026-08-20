using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;

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
        //An impact area set is required
        List<ImpactAreaElement> impactAreaSet = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        if (impactAreaSet.Count == 0)
        {
            MessageBox.Show("An impact area set is required to import stage-life loss functions.", "Impact Area Set Required", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        EditorActionManager actionManager = new EditorActionManager().WithSiblingRules(this);
        LifeSimImporterVM vm = new(actionManager);
        DynamicTabVM tab = new("Import LifeSim", vm, "Import LifeSim");
        Navigate(tab, false, true);
    }
}
