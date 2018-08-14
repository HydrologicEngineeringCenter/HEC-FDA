using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:46:34 AM)]
    public class WaterSurfaceElevationOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:46:34 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public WaterSurfaceElevationOwnerElement(BaseFdaElement owner):base(owner)
        {
            Name = "Water Surface Elevations";
            IsBold = true;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            Utilities.NamedAction import = new Utilities.NamedAction();
            import.Header = "Import Water Surface Elevations";
            import.Action = ImportWaterSurfaceElevations;

            //Utilities.NamedAction importFromAscii = new Utilities.NamedAction();
            //importFromAscii.Header = "Import Inflow Outflow Relationship From ASCII";
            //importFromAscii.Action = ImportFromASCII;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(import);
            //localActions.Add(importFromAscii);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public void ImportWaterSurfaceElevations(object arg1, EventArgs arg2)
        {
            WaterSurfaceElevationImporterVM vm = new WaterSurfaceElevationImporterVM(this);
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasFatalError)
                {
                    //add the element
                    WaterSurfaceElevationElement ele = new WaterSurfaceElevationElement(vm.Name, vm.Description, vm.ListOfRelativePaths,vm.IsDepthGridChecked, this);

                    AddElement(ele);
                    //add a transaction for each file copied over?
                    //i need to make sure that there is the same number of original paths as new paths.
                    if(vm.ListOfOriginalPaths.Count == vm.ListOfRelativePaths.Count)
                    {
                        for(int i = 0;i<vm.ListOfRelativePaths.Count;i++)
                        {
                            //AddTransaction(new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.CreateNew, "File " + vm.ListOfOriginalPaths[i]+ " was saved to " + vm.ListOfRelativePaths[i], nameof(WaterSurfaceElevationElement)));

                        }
                    }

                }
            }

        }
        #endregion
        #region Functions
        #endregion
        public override string TableName
        {
            get
            {
                return "Water Surface Elevations";
            }
        }

        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }


        public override string[] TableColumnNames()
        {
            return new string[] { "Name","Description","IsDepthGrids" };
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string),typeof(bool) };
        }
        public override void AddElement(object[] rowData)
        {

            List<PathAndProbability> ppList = new List<PathAndProbability>();


            WaterSurfaceElevationElement wse = new WaterSurfaceElevationElement((string)rowData[0], (string)rowData[1], ppList, (bool)rowData[2], this);

            int lastRow = Storage.Connection.Instance.GetTable(wse.TableName).NumberOfRows - 1;
            foreach (object[] row in Storage.Connection.Instance.GetTable(wse.TableName).GetRows(0, lastRow))
            {
                wse.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
            }

            AddElement(wse, false);
        }
    }
}
