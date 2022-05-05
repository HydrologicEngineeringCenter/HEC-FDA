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
        #region Constructors
        public ExteriorInteriorOwnerElement( ) : base()
        {
            Name = StringConstants.EXTERIOR_INTERIOR_FUNCTIONS;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

           NamedAction addExteriorInterior = new NamedAction();
            addExteriorInterior.Header = StringConstants.CREATE_EXT_INT_MENU;
            addExteriorInterior.Action = CreateNewExteriorInteriorCurve;

           NamedAction ImportFromAscii = new NamedAction();
            ImportFromAscii.Header = StringConstants.ImportFromOldFda(StringConstants.IMPORT_EXT_INT_FROM_OLD_NAME);
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
            string header = StringConstants.IMPORT_EXT_INT_FROM_OLD_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_EXT_INT_FROM_OLD_HEADER);
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
            string header = StringConstants.CREATE_EXT_INT_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_EXT_INT_HEADER);
            Navigate(tab, false, true);
        }

        #endregion
    }
}
