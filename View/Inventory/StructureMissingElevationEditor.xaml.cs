using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Inventory
{

    public partial class StructureMissingElevationEditor : UserControl
    {
        public StructureMissingElevationEditor()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(combined_text(sender as TextBox, e.Text));
        }

        public static bool IsValid(string str)
        {
            return double.TryParse(str, out double i);
        }

        private string combined_text(TextBox tb, string newText)
        {
            return tb.Text.Substring(0, tb.SelectionStart) + newText + tb.Text.Substring(tb.SelectionStart + tb.SelectionLength);
        }
    }
}
