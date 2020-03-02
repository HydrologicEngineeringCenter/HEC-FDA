using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public class UniformControlVM : IValueUncertainty
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public UniformControlVM(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public IOrdinate CreateOrdinate()
        {
            return IDistributedOrdinateFactory.FactoryUniform(Min, Max);
        }
    }
}
