using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;

namespace FdaViewModel.InflowOutflow
{
    class InflowOutflowOwnedElement : Utilities.OwnedElement
    {
        #region Notes
        #endregion
        #region Fields
        private string _Description ="";
        private UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
        public override string TableName
        {
            get
            {
                return "InflowOutflow - " + Name; // throw new NotImplementedException();
            }
        }

        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }
        public UncertainCurveDataCollection InflowOutflowCurve { get { return _Curve; } set { _Curve = value; NotifyPropertyChanged(); } }
        #endregion
        #region Constructors
        public InflowOutflowOwnedElement(string userprovidedname, string desc, Statistics.UncertainCurveDataCollection inflowOutflowCurve, BaseFdaElement owner) : base(owner)
        {
            Name = userprovidedname;
            InflowOutflowCurve = inflowOutflowCurve;
            Description = desc;
            if (Description == null) Description = "";
            Utilities.NamedAction editInflowOutflowCurve = new Utilities.NamedAction();
            editInflowOutflowCurve.Header = "Edit Inflow Outflow Curve";
            editInflowOutflowCurve.Action = EditInflowOutflowCurve;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editInflowOutflowCurve);

            Actions = localActions;
        }

        private void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Voids 
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        public override object[] RowData()
        {
            return new object[] { Name, Description };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
        #endregion
        #region Functions
        #endregion





    }
}
