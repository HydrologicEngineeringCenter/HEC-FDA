using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Alternatives;

namespace ViewModel.AlternativeComparisonReport
{
    public class ComparisonRowItemVM : BaseViewModel
    {
        private AlternativeElement _SelectedAlternative;

        public AlternativeElement SelectedAlternative
        {
            get { return _SelectedAlternative; }
            set { _SelectedAlternative = value;NotifyPropertyChanged(); }
        }
        public List<AlternativeElement> Alternatives { get; } = new List<AlternativeElement>();
        public ComparisonRowItemVM(List<AlternativeElement> elems)
        {
            Alternatives.AddRange(elems);
            if(elems.Count>0)
            {
                SelectedAlternative = elems[0];
            }
        }

    }
}
