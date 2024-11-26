using System.Windows.Controls;

namespace HEC.FDA.View.Results
{
    /// <summary>
    /// Interaction logic for ScenarioDamageSummary.xaml
    /// </summary>
    public partial class ScenarioDamageSummary : UserControl
    {
        public ScenarioDamageSummary()
        {
            InitializeComponent();
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "C2";
            }

        }
    }
}
