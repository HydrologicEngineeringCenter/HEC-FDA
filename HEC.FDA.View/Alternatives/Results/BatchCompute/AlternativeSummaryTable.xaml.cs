using HEC.FDA.ViewModel.Alternatives.Results.BatchCompute;
using System.Windows.Controls;

namespace HEC.FDA.View.Alternatives.Results.BatchCompute;

/// <summary>
/// Interaction logic for AlternativeSummaryTable.xaml
/// </summary>
public partial class AlternativeSummaryTable : UserControl
{
    public AlternativeSummaryTable()
    {
        InitializeComponent();
    }
    // This hits after autogenerating column in the FdaDataGrid, so we can override defalts here. 
    private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(AlternativeDamageRowItem.DiscountRate)))
        {
            (e.Column as DataGridTextColumn).Binding.StringFormat = "N4";
        }
        else if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
        {
            (e.Column as DataGridTextColumn).Binding.StringFormat = "C2";
        }
    }
}
