using HEC.FDA.ViewModel.Saving;
using System;
using System.Collections.ObjectModel;
using paireddata;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class ChildElement : BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields

        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);


        //private object _CustomTreeViewHeader;
        private string _Description = "";
        private UncertainPairedData _Curve;
        private bool _IsExpanded = true;
        private int _FontSize = 14;
        private bool _IsBold = false;

        #endregion
        #region Properties


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
        public UncertainPairedData Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }


        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        
        #endregion
        #region Constructors
        public ChildElement()
        {
            
        }
        #endregion
        /// <summary>
        /// Gets the elements ID by finding this elements persistence manager and using
        /// it's table name and element name to grab the ID from the database. -1 means
        /// it did not find it for some reason.
        /// </summary>
        /// <returns></returns>
        public int GetElementID()
        {
            IElementManager elementManager = PersistenceFactory.GetElementManager(this);
            if(elementManager is SavingBase)
            {
                SavingBase baseManager = ((SavingBase)elementManager);
                int id = baseManager.GetElementId(baseManager.TableName, Name);
                return id;
            }
            return -1;
        }

        public virtual void Rename(object sender, EventArgs e)
        {
            RenameVM renameViewModel = new RenameVM( this, CloneElement);

            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename");
            Navigate(tab);
        }  

        public abstract ChildElement CloneElement(ChildElement elementToClone);
        
        public virtual void RemoveElementFromMapWindow(object sender, EventArgs e) { }
     
    }
}
