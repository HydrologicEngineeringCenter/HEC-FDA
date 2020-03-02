using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    class OccupancyTypeEditable : IOccupancyTypeEditable
    {
        public IOccupancyType OccType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueUncertaintyVM StructureValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueUncertaintyVM ContentValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueUncertaintyVM VehicleValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueUncertaintyVM OtherValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ValueUncertaintyVM FoundationHeightUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
