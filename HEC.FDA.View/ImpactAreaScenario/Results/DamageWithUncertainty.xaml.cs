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

            if (e.Column is DataGridTextColumn textColumn)
            {
                textColumn.MinWidth = 96;
            }
        }
    }
}
