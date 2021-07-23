using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AggregatedStageDamage
{
    public enum StageDamageAssetType
    {
        //I am matching the enum that is in the Importer project 
        //StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
        STRUCTURE,
        CONTENT,
        OTHER,
        CAR,
        TOTAL
    }
}
