using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class LogNormalControlVM : ValidatingBaseViewModel, IValueUncertainty
    {
        private LogNormal _LogNormal;
        public event EventHandler WasModified;

        public bool DisplayMean { get; set; }
        public double StandardDeviation
        {
            get { return _LogNormal.StandardDeviation; }
            set
            {
                _LogNormal.StandardDeviation = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Mean
        {
            get { return _LogNormal.Mean; }
            set
            {
                _LogNormal.Mean = value;
                NotifyPropertyChanged();
                WasModified?.Invoke(this, new EventArgs());
            }
        }

        public string LabelString { get; set; }
        public LogNormalControlVM(double mean, double stDev, string labelString, bool displayMean = false)
        {
            DisplayMean = displayMean;
            LabelString = labelString;
            _LogNormal = new LogNormal(mean, stDev);
        }

        public ContinuousDistribution CreateOrdinate()
        {
            return _LogNormal;
        }
    }
}
