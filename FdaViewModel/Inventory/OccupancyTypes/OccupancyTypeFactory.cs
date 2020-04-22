using Functions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
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
            ot.DamageCategory = new DamageCategory.DamageCategory(damCatName);
            
            //depth damage curves
            List<double> xs = new List<double>() { 0 };
            List<double> ys = new List<double>() { 0 };
            ot.StructureDepthDamageFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ot.ContentDepthDamageFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ot.VehicleDepthDamageFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ot.OtherDepthDamageFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);

            //value uncertainties
            ot.StructureValueUncertainty = new Constant(0);
            ot.ContentValueUncertainty = new Constant(0);
            ot.VehicleValueUncertainty = new Constant(0);
            ot.OtherValueUncertainty = new Constant(0);
            ot.FoundationHeightUncertainty = new Constant(0);

            //group id
            ot.GroupID = groupId;

            //use the group id to get the number of occtypes and give this one 
            //an occtype id
            ot.ID = Saving.PersistenceFactory.GetOccTypeManager().GetIdForNewOccType(groupId);

            return ot;
        }
    }
}
