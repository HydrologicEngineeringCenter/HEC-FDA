using System;
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

    }
}
