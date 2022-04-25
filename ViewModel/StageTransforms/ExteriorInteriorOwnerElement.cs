using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class ExteriorInteriorOwnerElement :ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public ExteriorInteriorOwnerElement( ) : base()
        {
            Name = "Exterior Interior Relationships";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

           NamedAction addExteriorInterior = new NamedAction();
            addExteriorInterior.Header = "Create New Exterior Interior Relationship";
            addExteriorInterior.Action = CreateNewExteriorInteriorCurve;

           NamedAction ImportFromAscii = new NamedAction();
            ImportFromAscii.Header = StringConstants.ImportFromOldFda("Exterior Interior Relationship");
            ImportFromAscii.Action = ImportFromASCII;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addExteriorInterior);
            localActions.Add(ImportFromAscii);

            Actions = localActions;

            StudyCache.ExteriorInteriorAdded += AddExteriorInteriorElement;
            StudyCache.ExteriorInteriorRemoved += RemoveExteriorInteriorElement;
            StudyCache.ExteriorInteriorUpdated += UpdateExteriorInteriorElement;
        }
        #endregion
        #region Voids
        private void UpdateExteriorInteriorElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportExteriorInteriorFromFDA1VM();
            string header = "Import Exterior Interior Curve";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportExteriorInteriorCurve");
            Navigate(tab, false, true);
        }

        public void CreateNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(StringConstants.EXT_INT, StringConstants.EXT_STAGE, StringConstants.INT_STAGE);
            computeComponentVM.SetPairedData(DefaultCurveData.ExteriorInteriorDefaultCurve());

            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM(computeComponentVM, actionManager);
            string header = "Create Exterior Interior";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateExteriorInterior");
            Navigate(tab, false, true);
        }

        #endregion
    }
}
