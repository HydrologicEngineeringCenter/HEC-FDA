using FdaViewModel.Editors;
using FdaViewModel.Saving;
using FdaViewModel.WaterSurfaceElevation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public abstract class ChildElement : BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields

        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);


        //private object _CustomTreeViewHeader;
        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        private bool _IsExpanded = true;
        private int _FontSize = 14;
        private bool _IsBold = false;

        #endregion
        #region Properties


        public ObservableCollection<FdaLogging.LogItem> Logs { get; set; }
        public bool IsOpenInTabOrWindow { get; set; }
        //public bool IsExpanded
        //{
        //    get { return _IsExpanded; }
        //    set { _IsExpanded = value; NotifyPropertyChanged(nameof(IsExpanded)); }
        //}
        public int FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
        }
        public bool IsBold
        {
            get { return _IsBold; }
            set { _IsBold = value; NotifyPropertyChanged(nameof(IsBold)); }
        }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        // public OwnerElement Owner { get; }
        //public object CustomTreeViewHeader
        //{
        //    get { return _CustomTreeViewHeader; }
        //    set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        //}
        #endregion
        #region Constructors
        public ChildElement()
        {
        }
        #endregion
        #region Voids
        public virtual void Rename(object sender, EventArgs e)
        {
            //ChildElement oldElement = ((NamedAction)sender).Element;
            //if (oldElement == null)
            //{
            //    //error message?
            //    return;
            //}
            //string oldName = Name;
            //List<BaseFdaElement> siblings = StudyCache.GetSiblings(elem);

            RenameVM renameViewModel = new RenameVM( this, CloneElement);
            //StudyCache.AddSiblingRules(renameViewModel, this);
           // renameViewModel.ParentGUID = this.GUID;
            //Navigate( renameViewModel, false, true, "Rename");
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename");
            Navigate(tab);
            //if (renameViewModel.WasCanceled == true)
            //{
            //    return;
            //}
            //else
            //{
            //   //// renameViewModel.ElementToSave.Name = renameViewModel.Name;
            //   // UpdateTreeViewHeader(Name);
            //   // ChildElement oldElement = renameViewModel.OldElement;
            //   // Saving.IPersistable savingManager = Saving.PersistenceFactory.GetElementManager(this, StudyCache);
            //   // if (savingManager != null)
            //   // {
            //   //     savingManager.SaveExisting(renameViewModel.OldElement,renameViewModel.ElementToSave, 0, oldName);
            //   // }
            //}
        }

     

        public abstract ChildElement CloneElement(ChildElement elementToClone);

        //    if (CheckForNameConflict(renameViewModel.Name) == true)
        //    {
        //        string newName;

        //        int i = 1;
        //        string prevname = Name;
        //        do
        //        {
        //            newName = prevname + "_" + i;
        //            i++;
        //        } while (CheckForNameConflict(newName));

        //        do
        //        {
        //            renameViewModel = new RenameVM(newName, this);
        //            Navigate(renameViewModel, true, true);
        //            if (renameViewModel.WasCanceled)
        //            {
        //                //user aborted
        //                return;
        //            }
        //        } while (CheckForNameConflict(renameViewModel.Name));

        //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(renameViewModel.Name, Utilities.Transactions.TransactionEnum.Rename, "Previous Name: " + Name, GetType().Name));
        //        string theNewName = renameViewModel.Name;
        //        string oldTableName = this.GetTableConstant() + this.Name;
        //        string newTableName = this.GetTableConstant() + renameViewModel.Name;
        //        DoTheRename(oldTableName, newTableName, theNewName);
        //    }

        //    else
        //    {
        //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(renameViewModel.Name, Utilities.Transactions.TransactionEnum.Rename, "Previous Name: " + Name, GetType().Name));
        //        string newName = renameViewModel.Name;
        //        string oldTableName = this.GetTableConstant() + this.Name;
        //        string newTableName = this.GetTableConstant() + renameViewModel.Name;
        //        DoTheRename(oldTableName, newTableName, newName);

        //    }

        

        //private void DoTheRename(string oldTableName, string newTableName , string newName)
        //{
        //    //string oldName = this.GetTableConstant() + this.Name;
        //    //string newName = this.GetTableConstant() + renameViewModel.Name;
        //    if (this.SavesToTable())
        //    {
        //        if (this.TableContainsGeoData == true)
        //        {
        //            Storage.Connection.Instance.RenameGeoPackageTable(oldTableName, newTableName);
        //        }
        //        else
        //        {
        //            Storage.Connection.Instance.RenameTable(oldTableName, newTableName);
        //        }              

        //    }
        //    //specialty code for individual elements
        //    if(this.GetType() == typeof(WaterSurfaceElevation.WaterSurfaceElevationElement))
        //    {
        //        string oldDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + this.Name;
        //        string newDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + newName;
        //       if (System.IO.Directory.Exists(oldDirectoryPath ))
        //        {
        //            System.IO.Directory.Move(oldDirectoryPath, newDirectoryPath);
        //        }
        //    }
        //    if (this.GetType() == typeof(GeoTech.LeveeFeatureElement))
        //    {
        //        List<GeoTech.FailureFunctionElement> ffList = GetElementsOfType<GeoTech.FailureFunctionElement>();

        //        foreach (GeoTech.FailureFunctionElement ffe in ffList)
        //        {
        //            if (ffe.SelectedLateralStructure.Name == Name)
        //            {
        //                ffe.SelectedLateralStructure.Name = newName;
        //            }
        //        }
        //        List<GeoTech.FailureFunctionOwnerElement> dummyList = GetElementsOfType<GeoTech.FailureFunctionOwnerElement>();
        //        if (dummyList.FirstOrDefault() != null)
        //        {
        //            dummyList.FirstOrDefault().Save();
        //        }
        //    }


        //        this.Name = newName;
        //    ((ParentElement)_Owner).UpdateParentTable();

        //}


        //private bool CheckForNameConflict(string newName)
        //{

        //    foreach (ChildElement o in ((ParentElement)_Owner).Elements)
        //    {
        //        if (o.Name.Equals(newName)) { return true; }
        //    }
        //    return false;

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldCurve"></param>
        /// <param name="newCurve"></param>
        //public void UpdateTableIfModified(string oldName, Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        //{
        //    bool isModified = AreCurvesDifferent(oldCurve, newCurve);
        //    if(isModified && SavesToTable())
        //    {
        //        //if the name has changed then we need to delete the old table
        //        Storage.Connection.Instance.DeleteTable(TableName);
        //        Save();
        //    }
        //}
        /// <summary>
        ///this is basically just checking if the two are equal. This would be better if there was an overriden equals method on the objects themselves
        /// </summary>
        /// <param name="oldCurve"></param>
        /// <param name="newCurve"></param>
        /// <returns></returns>
        //private bool AreCurvesDifferent(Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        //{
        //    bool isModified = false;
        //    if (oldCurve.Distribution != newCurve.Distribution) { isModified = true; }
        //    if (oldCurve.Count != newCurve.Count) { isModified = true; }
        //    if (oldCurve.GetType() != newCurve.GetType()) { isModified = true; }
        //    //are all x values the same
        //    for (int i = 0; i < oldCurve.XValues.Count(); i++)
        //    {
        //        if (oldCurve.XValues[i] != newCurve.XValues[i])
        //        {
        //            isModified = true;
        //            break;
        //        }
        //    }
        //    for (int i = 0; i < oldCurve.YValues.Count(); i++)
        //    {
        //        if (oldCurve.YValues[i] != newCurve.YValues[i])
        //        {
        //            isModified = true;
        //            break;
        //        }
        //    }
        //    return isModified;
        //}

        //private async void RemoveTerrainFileOnBackgroundThread(ParentElement o)
        //{
        //    try
        //    {
        //        await Task.Run(() =>
        //        {               
        //            System.IO.File.Delete(((Watershed.TerrainElement)this).GetTerrainPath());
        //        });          
        //    }
        //    catch(Exception e)
        //    {
        //        CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete terrain file: " + ((Watershed.TerrainElement)this).GetTerrainPath());
        //        Navigate(messageBox);
        //        CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
        //        return;
        //    }
        //    o.Elements.Remove(this);
        //    //delete associated table?
        //    RemoveElementFromTable();
        //    o.Save();
        //}

        //private void RemoveWaterSurfElev(ParentElement o)
        //{
        //    try
        //    {
        //        foreach(PathAndProbability pap in ((WaterSurfaceElevationElement)this).RelativePathAndProbability)
        //        {
        //            System.IO.File.Delete(Storage.Connection.Instance.HydraulicsDirectory + "\\" + pap.Path);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        CustomMessageBoxVM messageBox = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "Could not delete water surface elevation files");
        //        Navigate(messageBox);
        //        CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");
        //        return;
        //    }
        //    o.Elements.Remove(this);
        //    //delete associated table?
        //    RemoveElementFromTable();
        //    o.Save();
        //}


        //public virtual void Remove(object sender, EventArgs e)
        //{
        //    if (_Owner.GetType().BaseType == typeof(ParentElement))
        //    {
        //        Utilities.CustomMessageBoxVM vm = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK_Cancel, "Are you sure you want to delete this element?\r\n" + Name);
        //        Navigate(vm);
        //        if (vm.ClickedButton == CustomMessageBoxVM.ButtonsEnum.OK)
        //        {
        //            ParentElement o = (ParentElement)_Owner;
        //            this.RemoveElementFromMapWindow(this, new EventArgs());

        //            //special logic for deleting the terrain file from the study directory
        //            if (this.GetType() == typeof(Watershed.TerrainElement))
        //            {
        //                CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png", " -Deleting", true);
        //                this.Actions.Clear();
        //                RemoveTerrainFileOnBackgroundThread(o);
        //            }
        //            //special logic for deleting the terrain file from the study directory
        //            else if (this.GetType() == typeof(WaterSurfaceElevation.WaterSurfaceElevationElement))
        //            {
        //                //CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");
        //                //this.Actions.Clear();
        //                RemoveWaterSurfElev(o);
        //            }
        //            else
        //            {
        //                o.Elements.Remove(this);
        //                //delete associated table?
        //                RemoveElementFromTable();
        //                o.Save();
        //            }
        //        }

        //    }
        //}

        //private void RemoveElementFromTable()
        //{
        //    if (this.SavesToTable())
        //    {
        //        if (Storage.Connection.Instance.TableNames().Contains(TableName))
        //        {
        //            Storage.Connection.Instance.DeleteTable(TableName);
        //        }
        //    }
        //    AddTransaction(this, new Transactions.TransactionEventArgs(Name, Transactions.TransactionEnum.Delete, "", GetType().Name));
        //}

        #endregion
        #region Functions
        //public  BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        //{
        //    if(GetType()==t & Name.Equals(name))
        //    {
        //        return this;
        //    }else
        //    {
        //        return null;// _Owner.GetElementOfTypeAndName(t,name);
        //    }
        //}
        //public  List<T> GetElementsOfType<T>()
        //{
        //    return null;// _Owner.GetElementsOfType<T>();
        //}
     
        //public virtual ChildElement GetPreviousElementFromChangeTable(int changeTableIndex)
        //{
        //    ChildElement prevElement = null;
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(this.ChangeTableName());
        //    if (changeTableIndex < changeTableView.NumberOfRows)
        //    {
        //        if (changeTableView != null)
        //        {
        //            if (!Storage.Connection.Instance.IsOpen)
        //            {
        //                Storage.Connection.Instance.Open();
        //            }
        //            object[] rowData = changeTableView.GetRow(changeTableIndex);
               
        //            prevElement = ((ParentElement)this._Owner).CreateElementFromRowData(rowData);

        //            Storage.Connection.Instance.Close();
        //        }
        //    }
        //    return prevElement;
        //}

        //public virtual ChildElement GetNextElementFromChangeTable(int changeTableIndex)
        //{
        //    ChildElement nextElement = null;
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(this.ChangeTableName());
        //    if (changeTableIndex >= 0)
        //    {
        //        if (changeTableView != null)
        //        {
        //            if (!Storage.Connection.Instance.IsOpen)
        //            {
        //                Storage.Connection.Instance.Open();
        //            }
        //            object[] rowData = changeTableView.GetRow(changeTableIndex);

        //            nextElement = ((ParentElement)this._Owner).CreateElementFromRowData(rowData);

        //            Storage.Connection.Instance.Close();
        //        }
        //    }
        //    return nextElement;
        //}

        //public virtual int ChangeIndex { get; set; }
        

        //public virtual string ChangeTableName()
        //{
        //    return GetTableConstant() + Name + "-ChangeTable";
        //}


        //public abstract object[] RowData();
        //public abstract bool SavesToRow();
        
        /// <summary>
        /// This exists so that the few elements that can be added/removed from the map window can override
        /// it and be removed when removing the element from the study.
        /// </summary>
        public virtual void RemoveElementFromMapWindow(object sender, EventArgs e) { }


        
        #endregion
    }
}
