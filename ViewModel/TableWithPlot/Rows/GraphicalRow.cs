using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class GraphicalRow : SequentialRow
    {
        private double _Confidence95 = 0;
        private double _Confidence05 = 0;

        [DisplayAsColumn("5% Confidence")]
        [DisplayAsLine("5% Confidence")]
        public double Confidence05
        {
            get
            {
                return _Confidence05;
            }
            set
            {
                _Confidence05 = 0;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Confidence95));
            }
        }

        [DisplayAsColumn("95% Confidence")]
        [DisplayAsLine("95% Confidence")]
        public double Confidence95 { 
            get
            {
                return _Confidence95;
            }
            set
            {
                _Confidence95 = 0;
                NotifyPropertyChanged(nameof(Confidence05));
                NotifyPropertyChanged();
            }
        }

        [DisplayAsColumn("Y Value", 1)]
        [DisplayAsLine("Y Value")]
        public double Value
        {
            get
            {
                return ((Deterministic)Y).Value;
            }
            set
            {
                Y = new Deterministic(value);
                Confidence05 = 0;
                Confidence95 = 0;
                NotifyPropertyChanged();
            }
        }
        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }
        public GraphicalRow(double x, double y, bool isMonotonicallyIncreasing = false) : base(x, new Deterministic(y), isMonotonicallyIncreasing)
        {
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (NextRow == null) return true; return Value < ((GraphicalRow)NextRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (PreviousRow == null) return true; return Value > ((GraphicalRow)PreviousRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            PropertyChanged += GraphicalRow_PropertyChanged;
        }

        public void GraphicalRow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Value) || e.PropertyName == nameof(X))
            {
                Confidence05 = 0;
                Confidence95 = 0;
                if (NextRow != null)
                {
                    if (((GraphicalRow)NextRow).Confidence05 != 0)
                    {
                        ((GraphicalRow)NextRow).Confidence05 = 0;
                    }
                }
                if (PreviousRow != null)
                {
                    if (((GraphicalRow)(PreviousRow)).Confidence05 != 0)
                    {
                        ((GraphicalRow)PreviousRow).Confidence05 = 0;
                    }
                }
            }
        }

        public void SetConfidenceLimits(double conf05, double conf95)
        {
            _Confidence95 = conf95;
            _Confidence05 = conf05;
            NotifyPropertyChanged(nameof(Confidence95));
            NotifyPropertyChanged(nameof(Confidence05));
        }
    }
}
