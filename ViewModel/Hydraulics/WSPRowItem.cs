using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Hydraulics
{
    class WSPRowItem : Consequences_Assist.DataGridRowItem
    {

        #region Notes
        #endregion
        #region Fields
        private double _Invert;
        #endregion
        #region Properties
        public double Station
        {
            get { return _Invert; }
            set { _Invert = value; NotifyPropertyChanged(); }
        }
        //[Consequences_Assist.Plottable("Invert","Station","Black",true)]
        public double Invert
        {
            get { return _Invert; }
            set { _Invert = value; NotifyPropertyChanged(); }
        }
        //public double Profile_One_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Two_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Three_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Four_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Five_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Six_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Seven_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        //public double Profile_Eight_Flow
        //{
        //    get { return _Invert; }
        //    set { _Invert = value; NotifyPropertyChanged(); }
        //}
        #endregion
        #region Constructors
        public WSPRowItem(IList<double> probs, IList<double> flows, IList<double> stages, ObservableCollection<object> list) : base(list)
        {
            //System.Dynamic.ExpandoObject dobj = this;
            dynamic dobj = this;
            dobj.asdf = "hello";

            string test = dobj.asdf;
            string test2 = dobj.lmnop;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override string PropertyDisplayName(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
