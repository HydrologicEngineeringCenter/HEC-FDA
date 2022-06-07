using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class StructureDamageResult
    {
        private double _StructureDamage;
        private double _ContentDamage;
        private double _OtherDamage;

        public double StructureDamage { get; set; }
        public double ContentDamage { get; set; }
        public double OtherDamage { get; set; }

        public StructureDamageResult(double structuredamage, double contentdamage, double otherdamage)
        {
            _StructureDamage = structuredamage;
            _ContentDamage = contentdamage;
            _OtherDamage = otherdamage;
        }

        public void AddResult(StructureDamageResult structureDamageResult)
        {
            _StructureDamage += structureDamageResult._StructureDamage;
            _ContentDamage += structureDamageResult._ContentDamage;
            _OtherDamage += structureDamageResult._OtherDamage;

        }
    }
}