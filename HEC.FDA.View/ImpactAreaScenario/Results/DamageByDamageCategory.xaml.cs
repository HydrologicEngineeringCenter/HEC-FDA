using HEC.FDA.ViewModel.Alternatives.Results;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactAreaScenario.Results
{
    /// <summary>
    /// Interaction logic for DamageByDamageCategory.xaml
    /// </summary>
    public partial class DamageByDamageCategory : UserControl
    {
        public DamageByDamageCategory()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DamageByDamCatVM vm)
            {
                //This feels a bit hacky but there is no good way to bind the column header. The UI doesn't consider the 
                //header to be part of the visual tree and therefore can't bind to the property in the VM.
                ead_textblock.Text = vm.EADLabel;
            }
        }
    }
}
