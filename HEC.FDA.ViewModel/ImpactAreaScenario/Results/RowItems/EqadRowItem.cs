using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
internal class EqadRowItem:IQuartileRowItem
{
    [DisplayAsColumn("Frequency")]
    public string Frequency { get; }
    [DisplayAsColumn(StringConstants.ALTERNATIVE_EqAD_LABEL)]
    public double Value { get; }

    public EqadRowItem(string frequency, double value)
    {
        Frequency = frequency;
        Value = value;
    }
}
