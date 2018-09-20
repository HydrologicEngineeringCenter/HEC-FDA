using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Watershed
{
    public class TerrainOwnerElement : Utilities.OwnerElement
    {
        public override string TableName
        {
            get
            {
                return "Terrains";
            }
        }

        #region Notes
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
        public TerrainOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = TableName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Import Terrain";
            add.Action = AddNew;
            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(add);
            Actions = localactions;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        private void AddNew(object arg1, EventArgs arg2)
        {
            TerrainBrowserVM vm = new TerrainBrowserVM(this);
            //vm.TerrainFileFinishedCopying += ReplaceTemporaryTerrainNode;
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasFatalError)
                {
                    //disable the context menu until the terrain is fully copied over and put a decorator on it
                    TerrainElement t = new TerrainElement(vm.TerrainName,System.IO.Path.GetFileName(vm.TerrainPath),this,true); // file extention?
                    //add to map window handler?
                    AddElement(t,false);//false-don't save this one


                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.TerrainName, Utilities.Transactions.TransactionEnum.CreateNew, "File " + vm.OriginalPath + " was saved as a terrain to " + vm.TerrainPath,nameof(TerrainElement)));
                }
            }
        }
        private void ReplaceTemporaryTerrainNode(object sender, EventArgs e)
        {
            TerrainBrowserVM vm = (TerrainBrowserVM)sender;
            string name = vm.TerrainName;

            //remove the temporary node and replace it
            foreach(Utilities.OwnedElement elem in Elements )
            {
                if(elem.Name.Equals(name))
                {
                    Elements.Remove(elem);
                    break;
                }
            }
            AddElement(new TerrainElement(name, System.IO.Path.GetFileName(vm.TerrainPath), this, false));

        }
        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }

        public override string[] TableColumnNames()
        {
            return new string[] { "Terrain Name", "Path Name" };
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string)};
        }
        public override object[] RowData()
        {
            throw new NotImplementedException();
        }

        public override bool SavesToRow()
        {
            return false;
        }

        public override void AddElement(object[] rowData)
        {
            AddElement(new TerrainElement((string)rowData[0], (string)rowData[1],this),false);
        }
    }
}
