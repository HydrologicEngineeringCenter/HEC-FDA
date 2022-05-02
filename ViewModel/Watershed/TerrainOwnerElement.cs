using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainOwnerElement : ParentElement
    {    

        #region Notes
        #endregion

        #region Constructors
        public TerrainOwnerElement( ) : base()
        {
            Name = StringConstants.TERRAIN;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            NamedAction add = new NamedAction();
            add.Header = StringConstants.IMPORT_TERRAIN_MENU;
            add.Action = AddNew;
            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(add);
            Actions = localactions;
            StudyCache.TerrainAdded += AddTerrainElement;
            StudyCache.TerrainRemoved += RemoveTerrainElement;
            StudyCache.TerrainUpdated += UpdateTerrainElement;
        }
        #endregion
        #region Voids
        private void UpdateTerrainElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void RemoveTerrainElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddTerrainElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        #endregion
       
        private void AddNew(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            TerrainBrowserVM vm = new TerrainBrowserVM( actionManager);
            string header = StringConstants.IMPORT_TERRAIN_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_TERRAIN_HEADER);
            Navigate( tab, false,true);
        }
    }
}
