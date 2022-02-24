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
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public InflowOutflowOwnerElement( ) : base()
        {
            Name = "Inflow Outflow Relationships";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addInflowOutflow = new NamedAction();
            addInflowOutflow.Header = "Create New Inflow Outflow Relationship...";
            addInflowOutflow.Action = AddInflowOutflow;

            NamedAction importInflowOutflow = new NamedAction();
            importInflowOutflow.Header = StringConstants.ImportFromOldFda("Inflow-Outflow");
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
            string header = "Import Inflow Outflow Curves";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportInflowOutflowCurve");
            Navigate(tab, false, true);
        }

        public void AddInflowOutflow(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
            ComputeComponentVM computeComponentVM = new ComputeComponentVM("Inflow-Outflow", "Inflow", "Outflow");
            Editors.InflowOutflowEditorVM vm = new Editors.InflowOutflowEditorVM(computeComponentVM, actionManager);

            string title = "Create Inflow Outflow";
            DynamicTabVM tab = new DynamicTabVM(title, vm, "NewInflowOutflow" + Name);
            Navigate( tab, false, false);
        }
      
        #endregion
    }
}
