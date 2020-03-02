using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public class TriangularControlVM : IValueUncertainty
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double MostLikely { get; set; }

        public TriangularControlVM(double mode, double min, double max)
        {
            MostLikely = mode;
            Min = min;
            Max = max;
        }

        public IOrdinate CreateOrdinate()
        {
            return IDistributedOrdinateFactory.FactoryTriangular(MostLikely, Min, Max);
        }
    }
}
