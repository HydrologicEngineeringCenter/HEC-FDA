using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Editors
{
    public class AnalyticalFrequencyCurveEditor: CurveEditorVM
    {
        private Statistics.LogPearsonIII _Distribution;
        private System.Collections.ObjectModel.ObservableCollection<object> _Items;
        private System.Collections.ObjectModel.ObservableCollection<double> _Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>();
        //private double _TestKnowledge = .9;
        //private double _TestNatural = .01;
        public Statistics.LogPearsonIII Distribution
        {
            get { return _Distribution; }
            set { _Distribution = value; NotifyPropertyChanged(); UpdateItems(); }
        }
        public System.Collections.ObjectModel.ObservableCollection<object> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(); }
        }
        public System.Collections.ObjectModel.ObservableCollection<double> Probabilities
        {
            get { return _Probabilities; }
            set { _Probabilities = value; NotifyPropertyChanged(); UpdateItems(); }
        }

        public double Mean { get { return _Distribution.GetMean; } set { Distribution = new Statistics.LogPearsonIII(value, _Distribution.GetStDev, _Distribution.GetG, _Distribution.GetSampleSize); NotifyPropertyChanged(); } }
        public double StandardDeviation { get { return _Distribution.GetStDev; } set { Distribution = new Statistics.LogPearsonIII(_Distribution.GetMean, value, _Distribution.GetG, _Distribution.GetSampleSize); NotifyPropertyChanged(); } }
        public double Skew { get { return _Distribution.GetG; } set { Distribution = new Statistics.LogPearsonIII(_Distribution.GetMean, _Distribution.GetStDev, value, _Distribution.GetSampleSize); NotifyPropertyChanged(); } }
        public int SampleSize { get { return _Distribution.GetSampleSize; } set { Distribution = new Statistics.LogPearsonIII(_Distribution.GetMean, _Distribution.GetStDev, _Distribution.GetG, value); NotifyPropertyChanged(); } }


        public AnalyticalFrequencyCurveEditor(Statistics.UncertainCurveDataCollection defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {

        }

        public AnalyticalFrequencyCurveEditor(ChildElement element, EditorActionManager actionManager) : base(element, actionManager)
        {
            
        }




        private void UpdateItems()
        {
            System.Collections.ObjectModel.ObservableCollection<object> tmp = new System.Collections.ObjectModel.ObservableCollection<object>();
            if (Distribution == null) return;
            if (_Probabilities.Count <= 0) return;
            List<double> probs = new List<double>();
            foreach (double d in _Probabilities)
            {
                probs.Add(d);
            }
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            //System.Diagnostics.Debug.Print(DateTime.Now.Millisecond.ToString());
            try
            {
                List<Statistics.Histogram> histos = Distribution.CreateConfidenceInterval(probs, .05, .95, .01, 10000);
                s.Stop();
                System.Diagnostics.Debug.Print(s.ElapsedMilliseconds.ToString());
                for (int i = 0; i < probs.Count; i++)
                {
                    tmp.Add(new AnalyticalFrequencyRowItem(tmp, _Probabilities[i], histos[i]));
                }
                Items = tmp;
            }
            catch (Exception ex)
            {
                s.Stop();
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("A value of mean standard deviation or skew was supplied that caused the confidence interval method to crash", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            }
        }


        public override void AddValidationRules()
        {
            //probs must be increasing?
            //skew limits?
            //variance limits?
            AddRule(nameof(Mean), () => Mean > 1, "Mean must be greater than 1");
            AddRule(nameof(Mean), () => Mean < 9, "Mean must be less than 9");
            AddRule(nameof(StandardDeviation), () => StandardDeviation > 0, "Standard Deviation must be greater than 0");
            AddRule(nameof(StandardDeviation), () => StandardDeviation < .5, "Standard Deviation must be less than .5");
            AddRule(nameof(Skew), () => Skew > -1.5, "Skew must be greater than -1.5");
            AddRule(nameof(Skew), () => Skew < 1.5, "Skew must be less than 1.5");

            AddRule(nameof(SampleSize), () => SampleSize > 5, "Sample size cannot be less than 5");
            AddRule(nameof(SampleSize), () => SampleSize < 300, "Sample size cannot be more than 300");

            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");

        }


    }
}
