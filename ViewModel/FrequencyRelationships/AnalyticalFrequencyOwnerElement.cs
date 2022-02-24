using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public AnalyticalFrequencyOwnerElement() : base()
        {
            Name = "Analyitical Flow Frequency Curves";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            NamedAction createNew = new NamedAction();
            createNew.Header = "Create New Analyitical Flow Frequency Curve...";
            createNew.Action = AddNewFlowFrequencyCurve;

            NamedAction importFlowFreq = new NamedAction();
            importFlowFreq.Header = StringConstants.ImportFromOldFda("Analyitical Flow Frequency");
            importFlowFreq.Action = ImportFlowFreqFromAscii;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(createNew);
            localActions.Add(importFlowFreq);

            Actions = localActions;

            StudyCache.FlowFrequencyAdded += AddFlowFrequencyElement;
            StudyCache.FlowFrequencyRemoved += RemoveFlowFrequencyElement;
            StudyCache.FlowFrequencyUpdated += UpdateFlowFrequencyElement;
        }

        #endregion
        #region Voids
        private void UpdateFlowFrequencyElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.NewElement);
        }
        private void RemoveFlowFrequencyElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddFlowFrequencyElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        private void ImportFlowFreqFromAscii(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportFrequencyFromFDA1VM();
            string header = "Import Frequency Curve";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportFrequencyCurve");
            Navigate(tab, false, true);
        }

        public void AddNewFlowFrequencyCurve(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
               .WithSiblingRules(this);

            ComputeComponentVM computeComponentVM = new ComputeComponentVM("Flow - Frequency", "Frequency", "Flow");
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(computeComponentVM, actionManager);
            string header = "Import Frequency";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportFrequency");
            Navigate(tab,false,false);
        }  
        #endregion
    }
}
