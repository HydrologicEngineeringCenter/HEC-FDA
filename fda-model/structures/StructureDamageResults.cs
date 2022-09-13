using structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fda_model.structures
{
    internal class StructureDamageResults
{
        private List<StructureDamageResult> _StructureDamageResults;


        public StructureDamageResults()
        {

        }

        public void AddResult(StructureDamageResult structureDamageResult)
        {
            StructureDamageResult structureDamage = GetDamageResult(structureDamageResult.DamageCategory);
            StructDamage += structureDamageResult.StructDamage;
            ContentDamage += structureDamageResult.ContentDamage;
            OtherDamage += structureDamageResult.OtherDamage;
            VehicleDamage += structureDamageResult.VehicleDamage;

        }

        private StructureDamageResult GetDamageResult(string damageCategory)
        {
            throw new NotImplementedException();
        }
    }
}
