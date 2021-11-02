using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class ResultRowItem:BaseViewModel
    {
        public string Name{get;set;}
        public SpecificIASResultVM Result { get; set; }
        public ResultRowItem(SpecificIASResultVM result)
        {
            Name = result.Name;
            Result = result;
        }

    }
}
