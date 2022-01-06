using paireddata;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public static class OccupancyTypeFactory
    {
        //public static IOccupancyType Factory(OccTypeReader.ComputableObjects.OccupancyType ot)
        //{
        //    OccupancyType retval = new OccupancyType(ot.Name, ot.DamageCategory.Name);
        //    //retval.Description = ot.Description;
        //    ////todo: i need to also translate the damcat
        //    //retval.CalculateStructureDamage = ot.CalcStructDamage;
        //    //retval.CalcualateContentDamage = ot.CalcContentDamage;
        //    //retval.CalculateVehicleDamage = ot.CalcVehicleDamage;
        //    //retval.CalculateOtherDamage = ot.CalcOtherDamage;

        //    //ot.getst

        //    //retval.StructureDepthDamageFunction = ot.GetStructurePercentDD()
        //    return retval;
        //}

        public static IOccupancyType Factory()
        {
            return new OccupancyType();
        }

        public static IOccupancyType Factory(string name, string selectedDamageCategoryName)
        {
            return new OccupancyType(name, selectedDamageCategoryName);
        }

        public static IOccupancyType Factory(string name, string damCatName, int groupId)
        {
            OccupancyType ot = new OccupancyType();
            ot.Name = name;
            //todo: default the occtype is selected values to false?
            ot.CalculateStructureDamage = true;
            ot.CalculateContentDamage = true;
            ot.CalculateVehicleDamage = true;
            ot.CalculateOtherDamage = false;

            //todo: what about damage category?
            ot.DamageCategory = damCatName;
            
            //depth damage curves
            List<double> xs = new List<double>() { 0 };
            List<double> ys = new List<double>() { 0 };

            UncertainPairedData defaultStruct = UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Stage", "Damage", "Occtype");
            UncertainPairedData defaultCont = UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Stage", "Damage", "Occtype");
            UncertainPairedData defaultVehicle = UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Stage", "Damage", "Occtype");
            UncertainPairedData defaultOther = UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Stage", "Damage", "Occtype");

            ot.StructureDepthDamageFunction = defaultStruct;
            ot.ContentDepthDamageFunction = defaultCont;
            ot.VehicleDepthDamageFunction = defaultVehicle;
            ot.OtherDepthDamageFunction = defaultOther;

            //value uncertainties
            ot.StructureValueUncertainty = new Deterministic(0);
            ot.ContentValueUncertainty = new Deterministic(0);
            ot.VehicleValueUncertainty = new Deterministic(0);
            ot.OtherValueUncertainty = new Deterministic(0);
            ot.FoundationHeightUncertainty = new Deterministic(0);

            //group id
            ot.GroupID = groupId;

            //use the group id to get the number of occtypes and give this one 
            //an occtype id
            ot.ID = Saving.PersistenceFactory.GetOccTypeManager().GetIdForNewOccType(groupId);

            return ot;
        }
    }
}
