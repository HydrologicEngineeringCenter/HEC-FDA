
using System;
using System.Collections.ObjectModel;
using Statistics;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyRowItem : Consequences_Assist.DataGridRowItem
    {

        #region Notes
        #endregion
        #region Fields
        private double _Probability;
        private Statistics.Histogram _histo;
        #endregion
        #region Properties

        public double Probability { get { return _Probability; } set { _Probability = value; NotifyPropertyChanged(); } }
        public double NintyFive { get { return Math.Round( _histo.ExceedanceValue(.95),2); }}
        public double SeventyFive { get { return Math.Round( _histo.ExceedanceValue(.75),2); }}
        //[Consequences_Assist.Plottable("Mean", "Probability", "Black", true)]
        public double Mean { get { return Math.Round(_histo.GetMean,2); }}
        //[Consequences_Assist.Plottable("25% CI", "Probability", "Blue", true)]
        public double TwentyFive { get { return Math.Round( _histo.ExceedanceValue(.25),2); }}
        //[Consequences_Assist.Plottable("5% CI", "Probability", "Red", true)]
        public double OhFive { get { return Math.Round(_histo.ExceedanceValue(.05),2); }}

        #endregion
        #region Constructors
        public AnalyticalFrequencyRowItem(ObservableCollection<object> list) : base(list)
        {
        }
        public AnalyticalFrequencyRowItem(ObservableCollection<object> list, double Prob, Statistics.Histogram histo) : base(list)
        {
            Probability = Prob;
            _histo = histo;
            if (_histo.IsConverged && !_histo.HitIterationLimit)
            {
                System.Diagnostics.Debug.Print("Probability " + Prob + " converged");
            }
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        //private double ConfidenceIntervals(double ciVal)
        //{
        //    Statistics.Normal n = new Statistics.Normal();
        //    double K = (2 / _Distribution.GetG) * (Math.Pow(((n.getDistributedVariable(_Probability) - _Distribution.GetG / 6) * _Distribution.GetG / 6 + 1), 3.0d) - 1);
        //    double z = n.getDistributedVariable(ciVal);
        //    double Avalue = (1 - ((Math.Pow(z ,2)) / (2 * (_Distribution.GetSampleSize - 1))));
        //    double Bvalue = (Math.Pow(K, 2)) - ((Math.Pow(z, 2)) / (_Distribution.GetSampleSize));
        //    double rootvalue = Math.Pow(((Math.Pow(K, 2)) - (Avalue * Bvalue)), (1 / 2));
        //    if(ciVal> .5)
        //    {
        //        return Math.Pow(10,_Distribution.GetMean + (((K + rootvalue) / Avalue) * _Distribution.GetStDev));
        //    }
        //    else
        //    {
        //        return Math.Pow(10,_Distribution.GetMean + (((K - rootvalue) / Avalue) * _Distribution.GetStDev));
        //    }
        //}
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override string PropertyDisplayName(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Mean):
                    return "Mean";
                case nameof(OhFive):
                    return "0.05";
                case nameof(SeventyFive):
                    return "0.75";
                case nameof(NintyFive):
                    return "0.95";
                case nameof(TwentyFive):
                    return "0.25";
                default:
                    return propertyName;
            }
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            return true;
        }

    }
}
