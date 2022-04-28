using System;
using System.Windows;

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
        public string LastEditDate { get; set; }
        public int ID { get; }
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

        public virtual void RemoveElement(object sender, EventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetElementManager(this).Remove(this);
            }
        }

        /// <summary>
        /// I think this is only being used for renaming elements.
        /// </summary>
        /// <param name="elementToClone"></param>
        /// <returns></returns>
        public abstract ChildElement CloneElement(ChildElement elementToClone);

    }
}
