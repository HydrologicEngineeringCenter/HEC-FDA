using HEC.FDA.ViewModel.Alternatives.Results;
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
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "C2";
            }
        }
    }
}
