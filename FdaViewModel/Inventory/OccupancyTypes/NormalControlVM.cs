using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public class NormalControlVM : IValueUncertainty
    {
        public double StDev
        {
            get;
            set;
        }
        public double Mean { get; set; }

        public NormalControlVM(double mean, double stDev)
        {
            Mean = mean;
            StDev = stDev;
        }

        public IOrdinate CreateOrdinate()
        {
            return IDistributedOrdinateFactory.FactoryNormal(Mean, StDev);
        }
    }
}
