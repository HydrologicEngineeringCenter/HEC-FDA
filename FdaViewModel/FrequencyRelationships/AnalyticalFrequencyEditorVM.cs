using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Statistics;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM : Utilities.Transactions.TransactionAndMessageBase, Utilities.ISaveUndoRedo
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name;
        private string _Description;
        private Statistics.LogPearsonIII _Distribution;
        private System.Collections.ObjectModel.ObservableCollection<object> _Items;
        private System.Collections.ObjectModel.ObservableCollection<double> _Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>();
        private double _TestKnowledge = .9;
        private double _TestNatural = .01;

        #endregion
        #region Properties
        public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }

      
       
     

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
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
        public double TestKnowledge { get { return _TestKnowledge; } set { _TestKnowledge = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Result)); } }
        public double TestNatural { get { return _TestNatural; } set { _TestNatural = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Result)); } }
        public double Result { get { return _Distribution.GetG; } }

        public OwnedElement CurrentElement { get; set; }

       
        #endregion
        #region Constructors
        public AnalyticalFrequencyEditorVM() : base()
        {
            Distribution = new Statistics.LogPearsonIII(4, .4, .5, 50);
            Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
          
        }
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem ):base(elem)// string name, Statistics.LogPearsonIII lpiii, string description, Utilities.OwnerElement owner) : base()
        {
            //OriginalName = elem.Name;
            CurrentElement = elem;
            CurrentElement.ChangeIndex = 0;
            Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
            AssignValuesFromElementToEditor(elem);

            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);

        }
        #endregion
        #region Voids
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
        #endregion
        #region Functions
        #endregion
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

            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } } , "Name cannot be blank");

        }
        public override void Save()
        {
            ////throw new NotImplementedException();
            //if (_savedElement != null)
            //{
            //    _savedElement.Remove(this, new EventArgs());
            //}
        }

        public override void Undo()
        {
            UndoElement(this);
        }
        public override void Redo()
        {
            RedoElement(this);
        }
        public override void SaveWhileEditing()
        {
            SaveAction(this);
        }

       

        public void AssignValuesFromElementToEditor(OwnedElement element)
        {
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)element;
            Name = elem.Name;
            LastEditDate = elem.LastEditDate;
            Description = elem.Description;
            Distribution = elem.Distribution;
            //Mean = prevElement.Distribution.GetMean;//???

        }
        public void AssignValuesFromEditorToCurrentElement()
        {
            CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            CurrentElement.Name = Name;
            ((AnalyticalFrequencyElement)CurrentElement).Description = Description;
            ((AnalyticalFrequencyElement)CurrentElement).Distribution = Distribution;
        }

        public UncertainCurveDataCollection GetTheElementsCurve()
        {
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII lp3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);
            Statistics.CurveIncreasing curve = lp3.GetOrdinatesFunction().Function;
            //return (UncertainCurveDataCollection)curve;
            throw new NotImplementedException();

        }

        public UncertainCurveDataCollection GetTheEditorsCurve()
        {
            //return Distribution;
            throw new NotImplementedException();

        }
        public void UpdateNameWithNewValue(string name)
        {
            Name = name;
        }

    }
}
