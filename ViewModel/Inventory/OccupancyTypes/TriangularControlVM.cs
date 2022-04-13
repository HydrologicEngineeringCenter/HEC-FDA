using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class TriangularControlVM : IValueUncertainty
    {
        public event EventHandler WasModified;
        private double _Min;
        private double _Max;
        private double _MostLikely;
        
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
        public double MostLikely
        {
            get { return _MostLikely; }
            set
            {
                _MostLikely = value;
                WasModified?.Invoke(this, new EventArgs());
            }
        }
        public string MinLabelString { get; set; }
        public string MaxLabelString { get; set; }
        public bool DisplayMostLikely { get; set; }

        public TriangularControlVM(double mode, double min, double max, string minLabelString, string maxLabelString, bool displayMostLikely = false)
        {
            DisplayMostLikely = displayMostLikely;
            MinLabelString = minLabelString;
            MaxLabelString = maxLabelString;
            MostLikely = mode;
            Min = min;
            Max = max;
        }

        public ContinuousDistribution CreateOrdinate()
        {
            return new Triangular(Min, MostLikely, Max);
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();          
            if (Max < Min)
            {
                vr.AddErrorMessage("Triangular distribution max cannot be less than min");
            }
            if (Max < MostLikely)
            {
                vr.AddErrorMessage("Triangular distribution max cannot be less than most likely");
            }
            if (Min > MostLikely)
            {
                vr.AddErrorMessage("Triangular distribution most likely cannot be less than min");
            }
            return vr;
        }
    }
}
