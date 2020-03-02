using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public interface IValueUncertainty
    {
        IOrdinate CreateOrdinate();
    }
}
