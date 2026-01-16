using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactAreaScenario.Results
{

    /// <summary>
    /// Interaction logic for DamageWithUncertainty.xaml
    /// </summary>
    public partial class DamageWithUncertainty : UserControl
    {
        public DamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == nameof(IQuartileRowItem.Value))
                return;

            if (e.Column is DataGridTextColumn textColumn)
            {
                switch (e.PropertyName)
                {
                    case nameof(IQuartileRowItem.Frequency):
                        textColumn.Width = 110;
                        break;
                    default:
                        textColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                        break;
                }
                textColumn.MinWidth = 80;
            }
        }
    }
}
