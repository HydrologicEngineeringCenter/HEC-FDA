using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class StructureDamageResult
    {
        public double OtherDamage { get; set; }
        public double StructDamage { get; set; }
        public double ContentDamage { get; set; }
        public double VehicleDamage { get; set; }

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