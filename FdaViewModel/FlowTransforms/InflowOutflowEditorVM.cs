using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:48:25 AM)]
    public class InflowOutflowEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 9:48:25 AM
        #endregion
        #region Fields
        private string _Name;
        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        private string _PlotTitle = "Inflow-Outflow Curve";
        #endregion
        #region Properties
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _PlotTitle; }
            set { _PlotTitle = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        //public bool HasFatalError { get; internal set; }

        #endregion
        #region Constructors
        public InflowOutflowEditorVM() : base()
        {
            double[] xs = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            //double[] ys = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(2000), new Statistics.None(3000), new Statistics.None(4000), new Statistics.None(5000), new Statistics.None(6000), new Statistics.None(7000), new Statistics.None(8000), new Statistics.None(9000), new Statistics.None(10000), new Statistics.None(11000) };

            Curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, false,Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }
        public InflowOutflowEditorVM(string name, string description, Statistics.UncertainCurveDataCollection curve) : base()
        {

            Name = name;
            Description = description;
            Curve = curve;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != "test", "Name cannot be test.",false);

        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
