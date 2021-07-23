using System;
using System.Collections.ObjectModel;

namespace FdaViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 27 / 2016 9:27:23 AM")]
    public class ImpactAreaRowItem : Consequences_Assist.DataGridRowItem
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/27/2016 9:27:23 AM
        #endregion
        #region Fields
        private string _Name;
        private double _IndexPoint;
        #endregion
        #region Properties
        public int ID { get; set; }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public double IndexPoint
        {
            get { return _IndexPoint; }
            set { _IndexPoint = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
 
        public ImpactAreaRowItem(int id, string dispName, double indPoint, ObservableCollection<object> list) : base(list)
        {
            ID = id;
            Name = dispName;
            IndexPoint = indPoint;
        }
        #endregion
        #region Voids

        public override string PropertyDisplayName(string propertyName)
        {
            return propertyName;
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            return true;
        }

        public override void AddValidationRules()
        {
        }
        #endregion
        #region Functions
        #endregion
    }
}
