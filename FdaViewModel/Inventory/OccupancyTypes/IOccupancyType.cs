using FdaViewModel.Inventory.DamageCategory;
using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FdaViewModel.Saving.PersistenceManagers.OccTypePersistenceManager;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyType
    {
        bool IsModified { get; set; }
        int GroupID { get; set; }
        int ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IDamageCategory DamageCategory { get; set; }

        bool CalculateStructureDamage { get; set; }
        bool CalculateContentDamage { get; set; }
        bool CalculateVehicleDamage { get; set; }
        bool CalculateOtherDamage { get; set; }

        ICoordinatesFunction StructureDepthDamageFunction { get; set; }

        ICoordinatesFunction ContentDepthDamageFunction { get; set; }
        ICoordinatesFunction VehicleDepthDamageFunction { get; set; }
        ICoordinatesFunction OtherDepthDamageFunction { get; set; }

        IOrdinate StructureValueUncertainty { get; set; }
        IOrdinate ContentValueUncertainty { get; set; }
        IOrdinate VehicleValueUncertainty { get; set; }
        IOrdinate OtherValueUncertainty { get; set; }
        IOrdinate FoundationHeightUncertainty { get; set; }
        
         ValueUncertaintyType StructureUncertaintyType { get; set; }
        ValueUncertaintyType ContentUncertaintyType { get; set; }
        ValueUncertaintyType VehicleUncertaintyType { get; set; }
        ValueUncertaintyType OtherUncertaintyType { get; set; }
        ValueUncertaintyType FoundationHtUncertaintyType { get; set; }




       
        //public SampledOccupancyType GenerateSampledOccupancyType(ref Random Randy);
        //public void LoadFromFDAInformation(StringBuilder occtype, int startdata, int parameter);
        //public string WriteToFDAString();
        //public XElement WriteToXElement();

        //public delegate void ReportMessageEventHandler(string message);


    }
}
