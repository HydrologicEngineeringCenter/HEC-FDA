using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Alternatives.Results
{
    /// <summary>
    /// Interaction logic for EADDamageWithUncertainty.xaml
    /// </summary>
    public partial class EADDamageWithUncertainty : UserControl
    {
        public EADDamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Hide the raw Value property (only show FormattedValue)
            if (e.PropertyName == nameof(IQuartileRowItem.Value))
            {
                e.Cancel = true;
                return;
            }

            // Hide RiskType column if all values are null (single risk type scenario)
            if (e.PropertyName == nameof(IQuartileRowItem.RiskType))
            {
                if (sender is DataGrid dataGrid && dataGrid.ItemsSource is System.Collections.IEnumerable items)
                {
                    bool allNull = items.Cast<IQuartileRowItem>().All(r => r.RiskType == null);
                    if (allNull)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            if (e.Column is DataGridTextColumn textColumn)
            {
                // Use SizeToCells so columns resize to fit content
                textColumn.Width = DataGridLength.SizeToCells;
                textColumn.MinWidth = 60;
            }
        }
    }
}
