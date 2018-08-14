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
        private string _Name;
        private List<Utilities.NamedAction> _Actions;
        private Utilities.CustomHeaderVM _CustomTreeViewHeader;

        #endregion
        #region Events
        public TransactionEventHandler TransactionEvent;
        #endregion
        #region Properties
        public Utilities.CustomHeaderVM CustomTreeViewHeader
        {
            get { return _CustomTreeViewHeader; }
            set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; UpdateTheTreeViewHeader(value); NotifyPropertyChanged(); }
        }
        public abstract string TableName { get; }
        public List<Utilities.NamedAction> Actions
        {
            get { return _Actions; }
            set { _Actions = value; NotifyPropertyChanged(nameof(Actions)); }
        }
        #endregion
        #region Constructors
        public BaseFdaElement()
        {
            PropertyChanged += BaseFdaElement_PropertyChanged;
        }

        private void BaseFdaElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CustomTreeViewHeader == null) { return; }
            if (e.PropertyName == nameof(HasError))
            {
                    if (HasError)
                    {
                        CustomTreeViewHeader.Decoration = " (!)";
                    }
                    else
                    {
                        CustomTreeViewHeader.Decoration = "";
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
       
        public void AddTransaction(object sender, Utilities.Transactions.TransactionEventArgs transaction)
        {
            TransactionEvent?.Invoke(this, transaction);
        }
        private void UpdateTheTreeViewHeader(string newName)
        {
            if (_CustomTreeViewHeader == null) { return; }
            string image = _CustomTreeViewHeader.ImageSource;
            string decoration = _CustomTreeViewHeader.Decoration;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(newName, image, decoration);
        }
        #endregion
        #region Functions
        public abstract string GetTableConstant();
        public abstract BaseFdaElement GetElementOfTypeAndName(Type t, string name);
        public abstract List<T> GetElementsOfType<T>() where T : Utilities.OwnedElement;
        //public abstract List<string> GetShapefiles
        #endregion

    }
}
