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
            ImportFromAscii.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_EXT_INT_FROM_OLD_NAME);
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
            string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_EXT_INT_FROM_OLD_NAME);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void CreateNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            CurveComponentVM curveComponentVM = DefaultData.ExteriorInteriorComputeComponent();

            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM(curveComponentVM, actionManager);
            string header = StringConstants.CREATE_EXT_INT_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_EXT_INT_HEADER);
            Navigate(tab, false, true);
        }

        #endregion
    }
}
