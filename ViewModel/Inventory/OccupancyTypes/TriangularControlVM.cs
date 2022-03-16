using Statistics;
using Statistics.Distributions;
using System;
using Utilities;

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
            //for now (8/10/20) we don't show the user the "MostLikely" value. We don't care
            //what it is, we just want it to always work so we will set it to be the same
            //as the min.
            MostLikely = Min;

            if (Min > 100 || Min < 0)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution min value needs to be between 0 and 100");
            }
            if (Max < 0)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution max value cannot be less than 0");
            }
            else
            {
                return new Triangular(MostLikely, Min, Max);
            }
        }
    }
}
