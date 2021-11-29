using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Inventory
{
    /// <summary>
    /// Interaction logic for StructureGroundElevationValidation_mockup.xaml
    /// </summary>
    public partial class StructureMissingElevationEditor : UserControl
    {
        public StructureMissingElevationEditor()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
            e.Handled = new Regex(@"^\-?[0 - 9] *\.?[0 - 9] +$").IsMatch(e.Text);

        }
    }
}
