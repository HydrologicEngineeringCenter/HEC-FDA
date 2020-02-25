using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Inventory.DamageCategory;
using Functions;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : IOccupancyType
    {
        public OccupancyType()
        {

        }
        public OccupancyType(string name, string damageCategoryName)
        {

        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IDamageCategory DamageCategory { get; set; }

        public bool CalculateStructureDamage { get; set; }

        public bool CalcualateContentDamage { get; set; }

        public bool CalculateVehicleDamage { get; set; }

        public bool CalculateOtherDamage { get; set; }

        public ICoordinatesFunction StructureDepthDamageFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction ContentDepthDamageFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction VehicleDepthDamageFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction OtherDepthDamageFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction StructureValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction ContentValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction VehicleValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction OtherValueUncertainty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICoordinatesFunction FoundationHeightUncertaintyFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string StructureDepthDamageName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentDepthDamageName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string VehicleDepthDamageName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string OtherDepthDamageName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IOccupancyType Clone()
        {
            throw new NotImplementedException();
        }
    }
}
