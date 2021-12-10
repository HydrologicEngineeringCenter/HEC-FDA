using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Alternatives;

namespace ViewModel.AlternativeComparisonReport
{
    public class AlternativeComboItem:BaseViewModel
    {
        private AlternativeElement _Alternative;
        private string _Name;
        public AlternativeElement Alternative { get { return _Alternative; } }

        /// <summary>
        /// I don't love storing the ID in here. The problem i am trying to solve is that if the user does 
        /// a name change, then i can't ask this element what its id is, because its name has already been 
        /// overwritten in the database. So i store it here while i can still get it.
        /// </summary>
        public int ID { get; }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        public AlternativeComboItem(AlternativeElement elem)
        {
            _Alternative = elem;
            Name = elem.Name;
            ID = elem.GetElementID();
        }

        public void UpdateAlternative(AlternativeElement elem)
        {
            _Alternative = elem;
            Name = elem.Name;
        }
    }
}
