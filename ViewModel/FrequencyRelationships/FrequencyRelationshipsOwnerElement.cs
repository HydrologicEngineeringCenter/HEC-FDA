using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class FrequencyRelationshipsOwnerElement : ParentElement
    {
        #region Constructors
        public FrequencyRelationshipsOwnerElement( ) : base()
        {
            Name = StringConstants.FREQUENCY_FUNCTIONS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            NamedAction createNew = new NamedAction();
            createNew.Header = StringConstants.CREATE_FREQUENCY_FUNCTIONS_MENU;
            createNew.Action = AddNewFlowFrequencyCurve;

            NamedAction importFlowFreq = new NamedAction();
            importFlowFreq.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_FREQUENCY_FROM_OLD_NAME);
            importFlowFreq.Action = ImportFlowFreqFromAscii;

            NamedAction importSyntheticFreq = new NamedAction();
            importSyntheticFreq.Header = StringConstants.IMPORT_SYNTHETIC_FUNTION_FROM_DBF;
            importSyntheticFreq.Action = CreateSyntheticFreqFromDBF;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(createNew);
            localActions.Add(importFlowFreq);
            localActions.Add(importSyntheticFreq);

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

        private void CreateSyntheticFreqFromDBF(object arg1, EventArgs arg2)
        {
            CreateSyntheticFrequencyFunctionVM vm = new CreateSyntheticFrequencyFunctionVM();
            string header = "Create Synthetic Frequency Function";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "synthetic");
            Navigate(tab, false, true);
        }

        private void ImportFlowFreqFromAscii(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportFrequencyFromFDA1VM();
            string header = StringConstants.CreateImportHeader(StringConstants.FREQUENCY_FUNCTIONS);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void AddNewFlowFrequencyCurve(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
               .WithSiblingRules(this);

            ComputeComponentVM computeComponentVM = new ComputeComponentVM(StringConstants.ANALYTICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(computeComponentVM, actionManager);
            string header = StringConstants.CREATE_FREQUENCY_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_FREQUENCY_HEADER);
            Navigate(tab, false, false);
        }
        #endregion
    }
}
