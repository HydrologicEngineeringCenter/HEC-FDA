using FdaViewModel.Inventory.DamageCategory;
using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    interface IOccupancyType
    {
        string Name { get; }

        string Description { get; }

        IDamageCategory DamageCategory { get; }

        bool CalculateStructureDamage { get; }
        bool CalcualateContentDamage { get; }
        bool CalculateVehicleDamage { get; }
        bool CalculateOtherDamage { get; }

        ICoordinatesFunction StructureDepthDamageFunction { get; }
        ICoordinatesFunction ContentDepthDamageFunction { get; }
        ICoordinatesFunction VehicleDepthDamageFunction { get; }
        ICoordinatesFunction OtherDepthDamageFunction { get; }



    }
}
