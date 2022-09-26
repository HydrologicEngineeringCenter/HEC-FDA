using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.FlowTransforms
{
    public class InflowOutflowOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Constructors
        public InflowOutflowOwnerElement( ) : base()
        {
            Name = StringConstants.REG_UNREG_TRANSFORM_FUNCTIONS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addInflowOutflow = new NamedAction();
            addInflowOutflow.Header = StringConstants.CREATE_REG_UNREG_MENU;
            addInflowOutflow.Action = AddInflowOutflow;

            NamedAction importInflowOutflow = new NamedAction();
            importInflowOutflow.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_REG_UNREG_FROM_OLD_NAME);
            importInflowOutflow.Action = ImportInflowOutflow;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addInflowOutflow);
            localActions.Add(importInflowOutflow);

            Actions = localActions;

            StudyCache.InflowOutflowAdded += AddInflowOutflowElement;
            StudyCache.InflowOutflowRemoved += RemoveInflowOutflowElement;
            StudyCache.InflowOutflowUpdated += UpdateInflowOutflowElement;
        }
        #endregion
        #region Voids
        private void UpdateInflowOutflowElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void ImportInflowOutflow(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportInflowOutflowFromFDA1VM();
            string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_REG_UNREG_FROM_OLD_NAME);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void AddInflowOutflow(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(StringConstants.REGULATED_UNREGULATED, StringConstants.UNREGULATED, StringConstants.REGULATED);
            computeComponentVM.SetPairedData(DefaultCurveData.RegulatedUnregulatedDefaultCurve());

            Editors.InflowOutflowEditorVM vm = new Editors.InflowOutflowEditorVM(computeComponentVM, actionManager);

            string title = StringConstants.CREATE_REG_UNREG_HEADER;
            DynamicTabVM tab = new DynamicTabVM(title, vm, StringConstants.CREATE_REG_UNREG_HEADER);
            Navigate( tab, false, false);
        }
      
        #endregion
    }
}
