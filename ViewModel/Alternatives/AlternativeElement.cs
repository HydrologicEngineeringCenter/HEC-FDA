using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario;
using ViewModel.Utilities;

namespace ViewModel.Alternatives
{
    public class AlternativeElement : ChildElement
    {
        public List<IASElementSet> IASElementSets { get; set; }

        public AlternativeElement(List<IASElementSet> IASElements)
        {
            IASElementSets = IASElements;
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            if(elementToClone is AlternativeElement)
            {
                AlternativeElement elem = (AlternativeElement)elementToClone;
            }
        }
    }
}
