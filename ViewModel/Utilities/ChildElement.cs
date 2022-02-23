using FdaLogging;
using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class ChildElement : BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields
        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);

        private string _Description = "";
        private int _FontSize = 14;
        private bool _IsBold = false;

        #endregion
        #region Properties

        public int ID { get; }
        public ObservableCollection<FdaLogging.LogItem> Logs { get; set; }
        public bool IsOpenInTabOrWindow { get; set; }

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

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public ChildElement(int id)
        {
            ID = id;
        }
        #endregion

        public virtual void Rename(object sender, EventArgs e)
        {
            RenameVM renameViewModel = new RenameVM(this, CloneElement);

            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename");
            Navigate(tab);
        }

        /// <summary>
        /// I think this is only being used for renaming elements.
        /// </summary>
        /// <param name="elementToClone"></param>
        /// <returns></returns>
        public abstract ChildElement CloneElement(ChildElement elementToClone);

        public virtual void RemoveElementFromMapWindow(object sender, EventArgs e) { }

        public override bool Equals(object obj)
        {
            bool retval = false;
            if (obj.GetType() == typeof(ChildElement))
            {
                retval = ID == ((ChildElement)obj).ID;
            }
            return retval;
        }

        public override int GetHashCode()
        {
            int hashCode = -975680090;
            hashCode = hashCode * -1521134295 + EqualityComparer<TransactionEventHandler>.Default.GetHashCode(TransactionEvent);
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + MinWidth.GetHashCode();
            hashCode = hashCode * -1521134295 + MinHeight.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LastEditDate);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Error);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ValidationErrorMessage);
            hashCode = hashCode * -1521134295 + HasError.GetHashCode();
            hashCode = hashCode * -1521134295 + HasFatalError.GetHashCode();
            hashCode = hashCode * -1521134295 + HasChanges.GetHashCode();
            hashCode = hashCode * -1521134295 + WasCanceled.GetHashCode();
            hashCode = hashCode * -1521134295 + IsExpanded.GetHashCode();
            hashCode = hashCode * -1521134295 + FontSize.GetHashCode();
            hashCode = hashCode * -1521134295 + IsBold.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ToolTip);
            hashCode = hashCode * -1521134295 + TableContainsGeoData.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<CustomHeaderVM>.Default.GetHashCode(CustomTreeViewHeader);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<NamedAction>>.Default.GetHashCode(Actions);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_Description);
            hashCode = hashCode * -1521134295 + _FontSize.GetHashCode();
            hashCode = hashCode * -1521134295 + _IsBold.GetHashCode();
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ObservableCollection<LogItem>>.Default.GetHashCode(Logs);
            hashCode = hashCode * -1521134295 + IsOpenInTabOrWindow.GetHashCode();
            hashCode = hashCode * -1521134295 + FontSize.GetHashCode();
            hashCode = hashCode * -1521134295 + IsBold.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }
    }
}
