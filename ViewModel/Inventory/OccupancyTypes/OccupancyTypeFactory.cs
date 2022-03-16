using paireddata;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Saving;

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

        //public static IOccupancyType Factory()
        //{
        //    return new OccupancyType();
        //}

        //public static IOccupancyType Factory(string name, string selectedDamageCategoryName)
        //{
        //    return new OccupancyType(name, selectedDamageCategoryName);
        //}

        private static OccTypeItem CreateDefaultItem(bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM("Stage-Damage", "Stage", "Damage");
            ContinuousDistribution structValueUncert = new Deterministic(0);
            return new OccTypeItem(isSelected, structureCurve, structValueUncert);
        }

        public static IOccupancyType Factory(string name, string damCatName, int groupId)
        {
            OccupancyType ot = new OccupancyType();
            ot.Name = name;
            ot.DamageCategory = damCatName;
            ot.Description = "";
            ot.GroupID = groupId;
            ot.StructureItem = CreateDefaultItem(true);
            ot.ContentItem = CreateDefaultItem(true);
            ot.VehicleItem = CreateDefaultItem(true);
            ot.OtherItem = CreateDefaultItem(false);
            ot.FoundationHeightUncertainty =  new Deterministic(0);
            ot.ContentToStructureValueUncertainty  = new Deterministic(0);
            ot.OtherToStructureValueUncertainty  = new Deterministic(0);

            OccTypePersistenceManager manager = PersistenceFactory.GetOccTypeManager();
            ot.ID = manager.GetNextAvailableId();
            //todo: maybe i don't have to do this?
            //manager.SaveNewOccType(ot);

            return ot;
        }
    }
}
