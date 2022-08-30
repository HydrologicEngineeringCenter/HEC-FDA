using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class StructureDamageResult
    {//TODO: how do we handle only some of these being populated? the way the code is written we're going to get weird stuff pumped in here 
        public double OtherDamage { get; set; }
        public double StructDamage { get; set; }
        public double ContentDamage { get; set; }
        public double VehicleDamage { get; set; }
        //TODO we probably want some other constructor so that all we do is add result iteratively and not worry about constructing with initial conditions 
        public StructureDamageResult(double structDamage, double contentDamage, double vehicleDamage, double otherDamage)
        {
            StructDamage = structDamage;
            ContentDamage = contentDamage;
            VehicleDamage = vehicleDamage;
            OtherDamage = otherDamage;
        }

        public void AddResult(StructureDamageResult structureDamageResult)
        {
            StructDamage += structureDamageResult.StructDamage;
            ContentDamage += structureDamageResult.ContentDamage;
            OtherDamage += structureDamageResult.OtherDamage;
            VehicleDamage += structureDamageResult.VehicleDamage;

        }
    }
}