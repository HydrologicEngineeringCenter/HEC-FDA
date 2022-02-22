using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainOwnerElement : ParentElement
    {    

        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties      
        #endregion
        #region Constructors
        public TerrainOwnerElement( ) : base()
        {
            Name = "Terrains";
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            NamedAction add = new NamedAction();
            add.Header = "Import Terrain";
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

            List<string> availableVRTPaths = new List<string>();
            ShapefilePaths(ref availableVRTPaths);
            TerrainBrowserVM vm = new TerrainBrowserVM(availableVRTPaths, actionManager);
            string header = "Import Terrain";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportTerrain");
            Navigate( tab, false,true);
        }
    }
}
