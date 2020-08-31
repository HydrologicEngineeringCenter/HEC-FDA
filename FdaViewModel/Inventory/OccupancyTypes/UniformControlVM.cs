using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;
using Utilities;

namespace FdaViewModel.Inventory.OccupancyTypes
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

        public IOrdinate CreateOrdinate()
        {
            //min should be 0 - 100
            if (Min > 100 || Min < 0)
            {
                throw new InvalidConstructorArgumentsException("Uniform distribution min value needs to be between 0 and 100");
            }
            if(Max < 0)
            {
                throw new InvalidConstructorArgumentsException("Uniform distribution max value cannot be less than 0");
            }
            else
            {
                return IDistributedOrdinateFactory.FactoryUniform(Min, Max);
            }
        }
    }
}
