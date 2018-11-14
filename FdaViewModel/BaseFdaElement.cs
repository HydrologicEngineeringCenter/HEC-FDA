using FdaViewModel.Editors;
using FdaViewModel.Study;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace FdaViewModel
{
    public delegate void TransactionEventHandler(object sender, Utilities.Transactions.TransactionEventArgs args);
    public abstract class BaseFdaElement : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        //private string _Name;
        private List<Utilities.NamedAction> _Actions;
        private Utilities.CustomHeaderVM _CustomTreeViewHeader;
        private bool _TableContainsGeoData = false;
        public Guid GUID { get; set; }


        #endregion
        #region Events
        public event EventHandler RenameMapTreeViewElement;
        public event EventHandler AddMapTreeViewElementBackIn;

        public void AddMapTreeViewItemBackIn(object sender, EventArgs e)
        {
            AddMapTreeViewElementBackIn?.Invoke(sender, e);
        }
        public void RenameMapTreeViewItem(object sender, EventArgs e)
        {
            RenameMapTreeViewElement?.Invoke(sender, e);
        }
        #endregion
        #region Properties


        public BaseFdaElement Parent { get; set; }
        public bool TableContainsGeoData
        {
            get { return _TableContainsGeoData; }
            set { _TableContainsGeoData = value; }
        }
        public Utilities.CustomHeaderVM CustomTreeViewHeader
        {
            get { return _CustomTreeViewHeader; }
            set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        }
        //public string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; UpdateTheTreeViewHeader(value); NotifyPropertyChanged(); }
        //}

        //public string LastEditDate
        //{
        //    get;set;
        //}
        //public abstract string TableName { get; }
        public List<Utilities.NamedAction> Actions
        {
            get { return _Actions; }
            set { _Actions = value; NotifyPropertyChanged(nameof(Actions)); }
        }

        //public abstract bool SavesToTable();
        #endregion
        #region Constructors
        public BaseFdaElement()
        {
            PropertyChanged += BaseFdaElement_PropertyChanged;
        }
        public BaseFdaElement(BaseFdaElement parent):base()
        {
            Parent = parent;
        }

        private void BaseFdaElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CustomTreeViewHeader == null) { return; }
            if (e.PropertyName == nameof(HasError))
            {
                    if (HasError)
                    {
                        //CustomTreeViewHeader.Decoration = " (!)";
                    }
                    else
                    {
                        //CustomTreeViewHeader.Decoration = "";
                    }

            }
            //else if(e.PropertyName == nameof(HasChanges))
            //{
            //    if(HasChanges == true)
            //    {
            //        CustomTreeViewHeader.Decoration = "*";
            //    }
            //    else
            //    {
            //        CustomTreeViewHeader.Decoration = "";
            //    }
            //}
        }
        #endregion
        #region Voids

        public void ExtendEventsToImporter(BaseViewModel vm)
        {
            vm.RequestNavigation += Navigate;
            vm.RequestShapefilePaths += ShapefilePaths;
            vm.RequestShapefilePathsOfType += ShapefilePathsOfType;
            vm.RequestAddToMapWindow += AddToMapWindow;
            vm.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            vm.TransactionEvent += AddTransaction;
        }
      
        public void UpdateTreeViewHeader(string newName)
        {
            if (_CustomTreeViewHeader == null) { return; }
            string image = _CustomTreeViewHeader.ImageSource;
            string decoration = _CustomTreeViewHeader.Decoration;
            bool gifVisible = _CustomTreeViewHeader.GifVisible;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(newName, image, decoration,gifVisible);
        }


        public void AssignValuesFromCurveEditorToElement(BaseEditorVM editorVM, ChildElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;
            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToCurveEditor(BaseEditorVM editorVM, ChildElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Curve;
        }

        #endregion
        #region Functions
        //public abstract string GetTableConstant();
        //public abstract BaseFdaElement GetElementOfTypeAndName(Type t, string name);
        //public abstract List<T> GetElementsOfType<T>() where T : Utilities.ChildElement;
        //public abstract List<string> GetShapefiles
        #endregion

    }
}
