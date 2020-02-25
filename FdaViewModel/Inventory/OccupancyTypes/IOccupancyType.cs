using FdaViewModel.Inventory.DamageCategory;
using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyType
    {
        string Name { get; set; }
        string Description { get; set; }
        IDamageCategory DamageCategory { get; set; }

        bool CalculateStructureDamage { get; }
        bool CalcualateContentDamage { get; }
        bool CalculateVehicleDamage { get; }
        bool CalculateOtherDamage { get; }

        ICoordinatesFunction StructureDepthDamageFunction { get; set; }

        ICoordinatesFunction ContentDepthDamageFunction { get; set; }
        ICoordinatesFunction VehicleDepthDamageFunction { get; set; }
        ICoordinatesFunction OtherDepthDamageFunction { get; set; }

        ICoordinatesFunction StructureValueUncertainty { get; set; }
        ICoordinatesFunction ContentValueUncertainty { get; set; }
        ICoordinatesFunction VehicleValueUncertainty { get; set; }
        ICoordinatesFunction OtherValueUncertainty { get; set; }
        ICoordinatesFunction FoundationHeightUncertaintyFunction { get; set; }
        


        string StructureDepthDamageName { get; set; }
        string ContentDepthDamageName { get; set; }
        string VehicleDepthDamageName { get; set; }
        string OtherDepthDamageName { get; set; }


        IOccupancyType Clone();
        //public SampledOccupancyType GenerateSampledOccupancyType(ref Random Randy);
        //public void LoadFromFDAInformation(StringBuilder occtype, int startdata, int parameter);
        //public string WriteToFDAString();
        //public XElement WriteToXElement();

        //public delegate void ReportMessageEventHandler(string message);


    }
}
