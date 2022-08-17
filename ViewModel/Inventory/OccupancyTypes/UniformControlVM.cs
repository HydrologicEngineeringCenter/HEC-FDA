using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using HEC.MVVMFramework.ViewModel.Validation;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class UniformControlVM : ValidatingBaseViewModel, IValueUncertainty
    {
        public event EventHandler WasModified;
        private Uniform _Uniform;

        public double Min
        {
            get { return _Uniform.Min; }
            set
            {
                _Uniform.Min = value;
                NotifyPropertyChanged();                             
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Max
        {
            get { return _Uniform.Max; }
            set
            {
                _Uniform.Max = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public string MinLabelString { get; set; }
        public string MaxLabelString { get; set; }

        public UniformControlVM(double min, double max, string minLabelString, string maxLabelString)
        {
            _Uniform = new Uniform(min, max);
            MinLabelString = minLabelString;
            MaxLabelString = maxLabelString;

            foreach(KeyValuePair<string, IPropertyRule> r in _Uniform.RuleMap)
            {
                RuleMap.Add(r.Key, r.Value);
            }          
        }

        public ContinuousDistribution CreateOrdinate()
        {
            return _Uniform;
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();           
            if (Min > Max)
            {
                //todo: should I just validate the _Uniform or even get rid of this?
                vr.AddErrorMessage("Uniform distribution max value cannot be less than min");
            }
            return vr;
        }
    }
}
