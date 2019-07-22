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
        //should this support bank?
        #endregion
        #region Properties
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
        public ImpactAreaRowItem():base(null)
        {
            Name = "";
            IndexPoint = -1;
        }
        public ImpactAreaRowItem(ObservableCollection<object> list) : base(list)
        {
            Name = "";
            IndexPoint = -1;
        }
        public ImpactAreaRowItem(string dispName, double indPoint, ObservableCollection<object> list) : base(list)
        {
            Name = dispName;
            IndexPoint = indPoint;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name == null, "Impact Area Name cannot be null.");
            //AddRule(nameof(Name), () => Name == "", "Impact Area Name cannot be null.");
            //AddRule(nameof(IndexPoint), () => IndexPoint < 0, "Index Point must be greater than or equal to zero.");
            //AddRule(nameof(IndexPoint), () => UniqueRule(nameof(IndexPoint), "Index Point must be unique."), "Index Point must be unique.");
            //AddRule(nameof(Name), () => UniqueRule(nameof(Name), "Impact Area Name must be unique."), "Impact Area Name must be unique.");
        }

        public override string PropertyDisplayName(string propertyName)
        {
            return propertyName;//throw new NotImplementedException();
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            return true;
        }
        #endregion
        #region Functions
        #endregion
    }
}
