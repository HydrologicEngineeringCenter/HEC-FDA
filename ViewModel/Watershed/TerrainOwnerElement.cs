using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainOwnerElement : Utilities.ParentElement
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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Import Terrain";
            add.Action = AddNew;
            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
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
            UpdateElement(e.OldElement, e.NewElement);
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
        #region Functions
        #endregion
       
        private void AddNew(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
                //.WithParentGuid(this.GUID)
                //.WithCanOpenMultipleTimes(true);

            List<string> availableVRTPaths = new List<string>();
            ShapefilePaths(ref availableVRTPaths);
            TerrainBrowserVM vm = new TerrainBrowserVM(availableVRTPaths, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //ExtendEventsToImporter(vm);
            //vm.AddSiblingRules(this);
            //vm.CanOpenMultipleTimes = true;
            //vm.ParentGUID = this.GUID;
            //vm.TerrainFileFinishedCopying += ReplaceTemporaryTerrainNode;
            string header = "Import Terrain";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportTerrain");
            Navigate( tab, false,true);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasFatalError)
            //    {
            //        //disable the context menu until the terrain is fully copied over and put a decorator on it
            //        TerrainElement t = new TerrainElement(vm.Name,System.IO.Path.GetFileName(vm.TerrainPath),this,true); // file extention?
            //        //add to map window handler?
            //        AddElement(t,false);//false-don't save this one


            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.CreateNew, "File " + vm.OriginalPath + " was saved as a terrain to " + vm.TerrainPath,nameof(TerrainElement)));
            //    }
            //}
        }
        private void ReplaceTemporaryTerrainNode(object sender, EventArgs e)
        {
            TerrainBrowserVM vm = (TerrainBrowserVM)sender;
            string name = vm.Name;

            //remove the temporary node and replace it
            foreach(Utilities.ChildElement elem in Elements )
            {
                if(elem.Name.Equals(name))
                {
                    Elements.Remove(elem);
                    break;
                }
            }
            AddElement(new TerrainElement(name, System.IO.Path.GetFileName(vm.TerrainPath), false));

        }
     

        //public override OwnedElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        //{
        //    TerrainBrowserVM editorVM = (TerrainBrowserVM)vm;
        //    //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
        //    //string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
        //    return new TerrainElement(editorVM.Name,editorVM.TerrainPath,this);
        //}

    }
}
