using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class LogNormalControlVM : IValueUncertainty
    {

        public event EventHandler WasModified;
        private double _StDev;
        private double _Mean;

        public bool DisplayMean { get; set; }
        public double StDev
        {
            get { return _StDev; }
            set
            {
                _StDev = value;
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public double Mean
        {
            get { return _Mean; }
            set
            {
                _Mean = value;
                WasModified?.Invoke(this, new EventArgs());
            }
        }

        public string LabelString { get; set; }
        public LogNormalControlVM(double mean, double stDev, string labelString, bool displayMean = false)
        {
            DisplayMean = displayMean;
            LabelString = labelString;
            Mean = mean;
            StDev = stDev;
        }

        public ContinuousDistribution CreateOrdinate()
        {
            return new LogNormal(Mean, StDev);
        }
    }
}
