using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem : IQuartileRowItem
    {
        [DisplayAsColumn(StringConstants.ALTERNATIVE_EAD_LABEL)]
        public string Frequency { get; }
        public double Value { get; }
        [DisplayAsColumn(StringConstants.QUARTILE_VALUE)]
        public string FormattedValue => Value.ToString("C2");

        public EadRowItem(string frequency, double value)
        {
            Frequency = frequency;
            Value = value;
        }

    }
}
