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
            if (Min > 100 || Min < 0)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution min value needs to be between 0 and 100");
            }
            if (Max < 0)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution max value cannot be less than 0");
            }
            else if(Max<Min)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution max cannot be less than min");
            }
            else if (Max < MostLikely)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution max cannot be less than most likely");
            }
            else if (Min > MostLikely)
            {
                throw new InvalidConstructorArgumentsException("Triangular distribution most likely cannot be less than min");
            }
            else
            {
                return new Triangular(Min, MostLikely, Max);
            }
        }
    }
}
