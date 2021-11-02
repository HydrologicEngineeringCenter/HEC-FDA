using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario;

namespace ViewModel.Alternatives
{
    public class AlternativeRowItem
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; }

        public IASElementSet Element { get; set; }
        public AlternativeRowItem(IASElementSet elem)
        {
            Element = elem;
            Name = elem.Name + " (" + elem.AnalysisYear + ")";
            IsSelected = false;
        }
    }
}
