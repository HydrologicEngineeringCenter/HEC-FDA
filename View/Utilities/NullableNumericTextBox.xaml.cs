using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for NullableNumericTextBox.xaml
    /// </summary>
    public partial class NullableNumericTextBox : UserControl
    {
        public static readonly DependencyProperty DoubleValueProperty = DependencyProperty.Register(nameof(DoubleValue), typeof(double), typeof(NullableNumericTextBox), new PropertyMetadata(DoubleValueChangedCallback));

        public double DoubleValue
        {
            get { return (double)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        public NullableNumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);

            //get datacontext and set the value?
        }

        private static void DoubleValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NullableNumericTextBox owner = (NullableNumericTextBox)d;
            
            double val = (double)e.NewValue;
            owner.txtBox.Text = val.ToString();
           
        }

    }
}
