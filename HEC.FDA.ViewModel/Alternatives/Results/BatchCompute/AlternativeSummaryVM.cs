using HEC.CS.Collections;
using HEC.FDA.ViewModel.Results;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{


    public class AlternativeSummaryVM : BaseViewModel
    {
        public List<AlternativeDamageRowItem> Rows { get; } = [];
        public List<AlternativeDamCatRowItem> DamCatRows { get; } = [];

        public AlternativeSummaryVM(List<AlternativeElement> altElems)
        {
            List<AlternativeDamCatRowItem> damCatRows = new List<AlternativeDamCatRowItem>();
            foreach (AlternativeElement element in altElems)
            {
                Rows.AddRange( AlternativeDamageRowItem.CreateAlternativeDamageRowItems(element));
                DamCatRows.AddRange( AlternativeDamCatRowItem.CreateAlternativeDamCatRowItems(element));
            }
        }



    }
}
