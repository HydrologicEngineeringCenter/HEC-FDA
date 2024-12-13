using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;
using System.Reflection.Metadata;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem:IQuartileRowItem
    {
        [DisplayAsColumn("Frequency")]
        public string Frequency { get; }
        [DisplayAsColumn(StringConstants.ALTERNATIVE_EAD_LABEL)]
        public double Value { get;  }

        public EadRowItem(string frequency, double value)
        {
            Frequency = frequency;
            Value = value;
        }

    }
}
