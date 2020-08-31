using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;
using Utilities;

namespace FdaViewModel.Inventory.OccupancyTypes
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
        public string LabelString { get; set; }

        public TriangularControlVM(double mode, double min, double max, string labelString)
        {
            LabelString = labelString;
            MostLikely = mode;
            Min = min;
            Max = max;
        }

        public IOrdinate CreateOrdinate()
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
                return IDistributedOrdinateFactory.FactoryTriangular(MostLikely, Min, Max);
            }
        }
    }
}
