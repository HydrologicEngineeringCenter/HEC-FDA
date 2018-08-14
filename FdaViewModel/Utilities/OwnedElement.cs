using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public abstract class OwnedElement : BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields
        protected BaseFdaElement _Owner;
        //private object _CustomTreeViewHeader;

        #endregion
        #region Properties
        //public object CustomTreeViewHeader
        //{
        //    get { return _CustomTreeViewHeader; }
        //    set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        //}
        #endregion
        #region Constructors
        public OwnedElement(BaseFdaElement owner)
        {
            _Owner = owner;
        }
        #endregion
        #region Voids
        public virtual void Rename(object sender, EventArgs e)
        {
            RenameVM renameViewModel = new RenameVM(Name);
            Navigate(renameViewModel, true, true, "Rename");
            if(renameViewModel.WasCancled == true) { return; }
            if (CheckForNameConflict(renameViewModel.Name) == true)
            {
                string newName;

                int i = 1;
                string prevname = Name;
                do
                {
                    newName = prevname + "_" + i;
                    i++;
                } while (CheckForNameConflict(newName));

                do
                {
                    renameViewModel = new RenameVM(newName);
                    Navigate(renameViewModel, true, true);
                    if (renameViewModel.WasCancled)
                    {
                        //user aborted
                        return;
                    }
                } while (CheckForNameConflict(renameViewModel.Name));

                AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(renameViewModel.Name, Utilities.Transactions.TransactionEnum.Rename, "Previous Name: " + Name, GetType().Name));
                string theNewName = renameViewModel.Name;
                string oldTableName = this.GetTableConstant() + this.Name;
                string newTableName = this.GetTableConstant() + renameViewModel.Name;
                DoTheRename(oldTableName, newTableName, theNewName);
            }

            else
            {
                AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(renameViewModel.Name, Utilities.Transactions.TransactionEnum.Rename, "Previous Name: " + Name, GetType().Name));
                string newName = renameViewModel.Name;
                string oldTableName = this.GetTableConstant() + this.Name;
                string newTableName = this.GetTableConstant() + renameViewModel.Name;
                DoTheRename(oldTableName, newTableName, newName);

            }

        }

        private void DoTheRename(string oldTableName, string newTableName , string newName)
        {
            //string oldName = this.GetTableConstant() + this.Name;
            //string newName = this.GetTableConstant() + renameViewModel.Name;
            if (this.SavesToTable())
            {
                if (this.TableContainsGeoData == true)
                {
                    Storage.Connection.Instance.RenameGeoPackageTable(oldTableName, newTableName);
                }
                else
                {
                    Storage.Connection.Instance.RenameTable(oldTableName, newTableName);
                }              

            }
            //specialty code for individual elements
            if(this.GetType() == typeof(WaterSurfaceElevation.WaterSurfaceElevationElement))
            {
                string oldDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + this.Name;
                string newDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + newName;
               if (System.IO.Directory.Exists(oldDirectoryPath ))
                {
                    System.IO.Directory.Move(oldDirectoryPath, newDirectoryPath);
                }
            }
            if (this.GetType() == typeof(GeoTech.LeveeFeatureElement))
            {
                List<GeoTech.FailureFunctionElement> ffList = GetElementsOfType<GeoTech.FailureFunctionElement>();

                foreach (GeoTech.FailureFunctionElement ffe in ffList)
                {
                    if (ffe.SelectedLateralStructure.Name == Name)
                    {
                        ffe.SelectedLateralStructure.Name = newName;
                    }
                }
                List<GeoTech.FailureFunctionOwnerElement> dummyList = GetElementsOfType<GeoTech.FailureFunctionOwnerElement>();
                if (dummyList.FirstOrDefault() != null)
                {
                    dummyList.FirstOrDefault().Save();
                }
            }


                this.Name = newName;
            ((OwnerElement)_Owner).UpdateParentTable();

        }


        private bool CheckForNameConflict(string newName)
        {

            foreach (OwnedElement o in ((OwnerElement)_Owner).Elements)
            {
                if (o.Name.Equals(newName)) { return true; }
            }
            return false;

        }

      

        public virtual void Remove(object sender, EventArgs e)
        {
            if (_Owner.GetType().BaseType == typeof(OwnerElement))
            {
                Utilities.CustomMessageBoxVM vm = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK_Cancel, "Are you sure you want to delete this element?\r\n" + Name);
                Navigate(vm);
                if (vm.ClickedButton == CustomMessageBoxVM.ButtonsEnum.OK)
                {
                    OwnerElement o = (OwnerElement)_Owner;
                    o.Elements.Remove(this);
                    //delete associated table?
                    if (this.SavesToTable())
                    {
                        if (Storage.Connection.Instance.TableNames().Contains(TableName))
                        {
                            Storage.Connection.Instance.DeleteTable(TableName);
                        }
                    }
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(Name, Utilities.Transactions.TransactionEnum.Delete, "",GetType().Name));
                    o.Save();
                }
                
            } else if (_Owner.GetType() == typeof(Study.FdaStudyVM))
            {
                //the sky is falling?
            }
            else
            {
                //the sky has fallen.
            }

            //check if this table exists in the sqlite file, and delete it.
            //FdaModel.Utilities.FdaFileManager.Instance.ProjectFile = SqliteFile;//not sure this is necessary...
            //if (FdaModel.Utilities.FdaFileManager.Instance.TableNames().Contains(TableName))
            //{
            //    FdaModel.Utilities.FdaFileManager.Instance.DeleteTable(TableName);
            //}
        }
        #endregion
        #region Functions
        public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        {
            if(GetType()==t & Name.Equals(name))
            {
                return this;
            }else
            {
                return _Owner.GetElementOfTypeAndName(t,name);
            }
        }
        public override List<T> GetElementsOfType<T>()
        {
            //List<BaseFdaElement> ret = new List<BaseFdaElement>();
            //if (GetType() == t)
            //{
            //    ret.Add(this);
            //    return ret;
            //}
            //else
            //{
            return _Owner.GetElementsOfType<T>();
            //}
        }
        public abstract object[] RowData();
        public abstract bool SavesToRow();
        public abstract bool SavesToTable();

       
        #endregion
    }
}
