using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    //[Author(q0heccdm, 11 / 3 / 2016 1:07:01 PM)]
    public class DatabaseTable:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/3/2016 1:07:01 PM
        #endregion
        #region Fields
        private System.Collections.ObjectModel.ObservableCollection<object> _Data;
        private string _Header;
        #endregion
        #region Properties
        public string Header
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(); }
        }
        public System.Collections.ObjectModel.ObservableCollection<object> Data
        {
            get { return _Data; }
            set { _Data = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public DatabaseTable(System.Collections.ObjectModel.ObservableCollection<object> listOfRowItems)
        {
            Data = listOfRowItems;
        }
        public DatabaseTable(string name, object[] colValues)
        {
            Header = name;
            Data = new System.Collections.ObjectModel.ObservableCollection<object>();
            foreach(object rv in colValues)
            {
                Data.Add(new ImpactArea.ImpactAreaRowItem(rv.ToString(), 0.0d, Data));
            }
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //
        }
        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion
    }
}
