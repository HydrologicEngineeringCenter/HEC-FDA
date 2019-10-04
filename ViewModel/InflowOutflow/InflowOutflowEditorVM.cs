using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.InflowOutflow
{
    public class InflowOutflowEditorVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Inflow Outflow";
        private Statistics.UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
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
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public InflowOutflowEditorVM() : base()
        {

            //double[] xValues = new double[] { 100, 200, 500, 1000, 2000 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(1), new Statistics.None(2), new Statistics.None(5), new Statistics.None(10), new Statistics.None(20) };
            double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }
        public InflowOutflowEditorVM(string name, string description, Statistics.UncertainCurveDataCollection curve) : base()
        {

            Name = name;
            Description = description;
            Curve = curve;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }
        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
