using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Utilities
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
            int i;
            return int.TryParse(str, out i) && i >= 0 && i <= 3000;
        }

        private string combined_text(TextBox tb, string new_text)
        {
            string s = tb.Text.Substring(0, tb.SelectionStart) + new_text + tb.Text.Substring(tb.SelectionStart + tb.SelectionLength);
            return s;
        }
    }
}
