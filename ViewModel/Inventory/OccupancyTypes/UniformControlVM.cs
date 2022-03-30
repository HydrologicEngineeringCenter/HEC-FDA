using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class UniformControlVM : IValueUncertainty
    {
        public event EventHandler WasModified;
        private double _Min;
        private double _Max;

        public double Min
        {
            get { return _Min; }
            set
            {
                _Min = value;
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Max
        {
            get { return _Max; }
            set
            {
                _Max = value;
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public string LabelString { get; set; }

        public UniformControlVM(double min, double max, string labelString)
        {
            LabelString = labelString;
            Min = min;
            Max = max;
        }

        public ContinuousDistribution CreateOrdinate()
        {
            //min should be 0 - 100
            return new Uniform(Min, Max);
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();           
            if (Min > Max)
            {
                vr.AddErrorMessage("Uniform distribution max value cannot be less than min");
            }
            return vr;
        }
    }
}
