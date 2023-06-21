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
            //link the plot with its chart view model
            DamageWithUncertaintyVM vm = (DamageWithUncertaintyVM)this.DataContext;
            frequency_textblock.Text = vm.ProbabilityExceedsValueLabel;
        }
        
    }
}
