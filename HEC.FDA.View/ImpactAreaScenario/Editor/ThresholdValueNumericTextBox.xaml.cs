using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace HEC.FDA.View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Limits the input to numeric values.
    /// </summary>
    public partial class ThresholdValueNumericTextBox : UserControl
    {
        public ThresholdValueNumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
    }
}
