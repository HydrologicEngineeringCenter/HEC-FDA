using System.Windows.Controls;
using System.Windows.Input;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for YearField.xaml
    /// </summary>
    public partial class YearField : UserControl
    {
        public YearField()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(combined_text(sender as TextBox, e.Text));
        }

        public static bool IsValid(string str)
        {
            return int.TryParse(str, out int i) && i >= 0 && i <= 3000;
        }

        private string combined_text(TextBox tb, string newText)
        {
            return tb.Text.Substring(0, tb.SelectionStart) + newText + tb.Text.Substring(tb.SelectionStart + tb.SelectionLength);
        }
    }
}
