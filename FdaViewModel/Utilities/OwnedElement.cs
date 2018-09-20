using FdaViewModel.WaterSurfaceElevation;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldCurve"></param>
        /// <param name="newCurve"></param>
        public void UpdateTableIfModified(string oldName, Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        {
            bool isModified = AreCurvesEqual(oldCurve, newCurve);
            if(isModified && SavesToTable())
            {
                //if the name has changed then we need to delete the old table
                Storage.Connection.Instance.DeleteTable(TableName);
                Save();
            }
        }
        /// <summary>
        ///this is basically just checking if the two are equal. This would be better if there was an overriden equals method on the objects themselves
        /// </summary>
        /// <param name="oldCurve"></param>
        /// <param name="newCurve"></param>
        /// <returns></returns>
        private bool AreCurvesEqual(Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        {
            bool isModified = false;
            if (oldCurve.Distribution != newCurve.Distribution) { isModified = true; }
            if (oldCurve.Count != newCurve.Count) { isModified = true; }
            if (oldCurve.GetType() != newCurve.GetType()) { isModified = true; }
            //are all x values the same
            for (int i = 0; i < oldCurve.XValues.Count(); i++)
            {
                if (oldCurve.XValues[i] != newCurve.XValues[i])
                {
                    isModified = true;
                    break;
                }
            }
            for (int i = 0; i < oldCurve.YValues.Count(); i++)
            {
                if (oldCurve.YValues[i] != newCurve.YValues[i])
                {
                    isModified = true;
                    break;
                }
            }
            return isModified;
        }

        private async void RemoveTerrainFileOnBackgroundThread(OwnerElement o)
        {
            try
            {
                await Task.Run(() =>
                {               
                    System.IO.File.Delete(((Watershed.TerrainElement)this).GetTerrainPath());
                });          
            }
            catch(Exception e)
            {
                CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete terrain file: " + ((Watershed.TerrainElement)this).GetTerrainPath());
                Navigate(messageBox);
                CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
                return;
            }
            o.Elements.Remove(this);
            //delete associated table?
            RemoveElementFromTable();
            o.Save();
        }

        private void RemoveWaterSurfElev(OwnerElement o)
        {
            try
            {
                foreach(PathAndProbability pap in ((WaterSurfaceElevationElement)this).RelativePathAndProbability)
                {
                    System.IO.File.Delete(Storage.Connection.Instance.HydraulicsDirectory + "\\" + pap.Path);
                }
                
            }
            catch (Exception e)
            {
                CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete water surface elevation files");
                Navigate(messageBox);
                CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");
                return;
            }
            o.Elements.Remove(this);
            //delete associated table?
            RemoveElementFromTable();
            o.Save();
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
                    this.RemoveElementFromMapWindow(this, new EventArgs());

                    //special logic for deleting the terrain file from the study directory
                    if (this.GetType() == typeof(Watershed.TerrainElement))
                    {
                        CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png", "...Deleting");
                        this.Actions.Clear();           
                        RemoveTerrainFileOnBackgroundThread(o);
                    }
                    //special logic for deleting the terrain file from the study directory
                    else if(this.GetType() == typeof(WaterSurfaceElevation.WaterSurfaceElevationElement))
                    {
                        //CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");
                        //this.Actions.Clear();
                        RemoveWaterSurfElev(o);
                    }
                    else
                    {
                        o.Elements.Remove(this);
                        //delete associated table?
                        RemoveElementFromTable();
                        o.Save();
                    }
                }
                
            } 
        }

        private void RemoveElementFromTable()
        {
            if (this.SavesToTable())
            {
                if (Storage.Connection.Instance.TableNames().Contains(TableName))
                {
                    Storage.Connection.Instance.DeleteTable(TableName);
                }
            }
            AddTransaction(this, new Transactions.TransactionEventArgs(Name, Transactions.TransactionEnum.Delete, "", GetType().Name));
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
        /// <summary>
        /// This exists so that the few elements that can be added/removed from the map window can override
        /// it and be removed when removing the element from the study.
        /// </summary>
        public virtual void RemoveElementFromMapWindow(object sender, EventArgs e) { }
       
        #endregion
    }
}
