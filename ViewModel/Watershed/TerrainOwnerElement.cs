using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;

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
        private void UpdateTerrainElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void RemoveTerrainElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddTerrainElement(object sender, ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        #endregion
       
        private void AddNew(object arg1, EventArgs arg2)
        {
            List<TerrainElement> terrainElems = StudyCache.GetChildElementsOfType<TerrainElement>();
            if (terrainElems.Count == 0)
            {

                Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                    .WithSiblingRules(this);

                TerrainBrowserVM vm = new TerrainBrowserVM(actionManager);
                string header = StringConstants.IMPORT_TERRAIN_HEADER;
                DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_TERRAIN_HEADER);
                Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show("Only one terrain is allowed.", "Terrain Already Exists", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
