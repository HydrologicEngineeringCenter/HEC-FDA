using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.GeoTech
{
    public class LeveeFeatureOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public LeveeFeatureOwnerElement( ) : base()
        {
            Name = StringConstants.LATERAL_STRUCTURES;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction add = new NamedAction();
            add.Header = StringConstants.CREATE_LATERAL_STRUCTURES_MENU;
            add.Action = AddNewLeveeFeature;

            NamedAction importFromFile = new NamedAction();
            importFromFile.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_LATERAL_STRUCTURES_FROM_OLD_NAME);
            importFromFile.Action = ImportFromFile;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(add);
            localActions.Add(importFromFile);

            Actions = localActions;

            StudyCache.LeveeAdded += AddLeveeElement;
            StudyCache.LeveeRemoved += RemoveLeveeElement;
            StudyCache.LeveeUpdated += UpdateLeveeElement;
        }
        #endregion
        #region Voids
        private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.NewElement);
        }
     
        private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        public void ImportFromFile(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportLeveeElementFromFDA1VM();
            string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_LATERAL_STRUCTURES_FROM_OLD_NAME);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void AddNewLeveeFeature(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);


            CurveComponentVM defaultCurve = DefaultData.DefaultLeveeComputeComponent();
            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(defaultCurve, actionManager);
            string header = StringConstants.CREATE_LATERAL_STRUCTURES_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_LATERAL_STRUCTURES_HEADER);
            Navigate(tab, false, false);       
        }
        #endregion
    }
}
