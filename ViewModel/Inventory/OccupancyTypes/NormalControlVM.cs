using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class NormalControlVM : ValidatingBaseViewModel, IValueUncertainty
    {
        private Normal _Normal;

        public event EventHandler WasModified;

        public bool DisplayMean { get; set; }
        public double StandardDeviation
        {
            get { return _Normal.StandardDeviation; }
            set
            {
                _Normal.StandardDeviation = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Mean
        {
            get { return _Normal.Mean; }
            set
            {
                _Normal.Mean = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }

        public string LabelString { get; set; }
        public NormalControlVM(double mean, double stDev, string labelString, bool displayMean = false)
        {
            DisplayMean = displayMean;
            LabelString = labelString;
            _Normal = new Normal(mean, stDev);
            foreach (KeyValuePair<string, IPropertyRule> r in _Normal.RuleMap)
            {
                RuleMap.Add(r.Key, r.Value);
            }
            if(displayMean)
            {
                AddSinglePropertyRule(nameof(Mean), new Rule(() => { return Mean >= 0; }, "Mean must be greater than or equal to 0.", MVVMFramework.Base.Enumerations.ErrorLevel.Severe));
            }

        }

        public ContinuousDistribution CreateOrdinate()
        {
            return _Normal;
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (StandardDeviation < 0)
            {
                vr.AddErrorMessage("Normal distribution standard deviation value cannot be less than 0");
            }
            return vr;
        }
    }
}
