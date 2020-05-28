using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:46:34 AM)]
    public class WaterSurfaceElevationOwnerElement : Utilities.ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:46:34 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
       
        #endregion
        #region Constructors
        public WaterSurfaceElevationOwnerElement( ):base()
        {
            Name = "Water Surface Elevations";
            IsBold = true;
            CustomTreeViewHeader = new CustomHeaderVM(Name);


            NamedAction import = new NamedAction();
            import.Header = "Import Water Surface Elevations";
            import.Action = ImportWaterSurfaceElevations;

            //Utilities.NamedAction importFromAscii = new Utilities.NamedAction();
            //importFromAscii.Header = "Import Inflow Outflow Relationship From ASCII";
            //importFromAscii.Action = ImportFromASCII;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(import);
            //localActions.Add(importFromAscii);

            Actions = localActions;

            StudyCache.WaterSurfaceElevationAdded += AddWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationRemoved += RemoveWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationUpdated += UpdateWaterSurfaceElevationElement;
        }
        #endregion
        #region Voids
        private void UpdateWaterSurfaceElevationElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void RemoveWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        public void ImportWaterSurfaceElevations(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(true);

            WaterSurfaceElevationImporterVM vm = new WaterSurfaceElevationImporterVM(actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            string header = "Import Water Surface Elevation";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportWatSurfElev");
            Navigate(tab, false,false);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasFatalError)
            //    {
            //        //add the element
            //        WaterSurfaceElevationElement ele = new WaterSurfaceElevationElement(vm.Name, vm.Description, vm.ListOfRelativePaths,vm.IsDepthGridChecked, this);

            //        AddElement(ele);
            //        //add a transaction for each file copied over?
            //        //i need to make sure that there is the same number of original paths as new paths.
            //        if(vm.ListOfOriginalPaths.Count == vm.ListOfRelativePaths.Count)
            //        {
            //            for(int i = 0;i<vm.ListOfRelativePaths.Count;i++)
            //            {
            //                //AddTransaction(new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.CreateNew, "File " + vm.ListOfOriginalPaths[i]+ " was saved to " + vm.ListOfRelativePaths[i], nameof(WaterSurfaceElevationElement)));

            //            }
            //        }

            //    }
            //}

        }
        #endregion
        #region Functions
        #endregion
       

      

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }


        
     
    }
}
