using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class NormalControlVM : IValueUncertainty
    {
        public event EventHandler WasModified;
        private double _StDev;
        private double _Mean;
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
        public NormalControlVM(double mean, double stDev, string labelString)
        {
            LabelString = labelString;
            Mean = mean;
            StDev = stDev;
        }

        public IDistribution CreateOrdinate()
        {
            return new Normal(Mean, StDev);
        }
    }
}
