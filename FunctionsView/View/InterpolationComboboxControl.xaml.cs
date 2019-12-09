using FunctionsView.ViewModel;
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

namespace FunctionsView.View
{
    /// <summary>
    /// Interaction logic for InterpolationComboboxControl.xaml
    /// </summary>
    public partial class InterpolationComboboxControl : UserControl
    {
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register("Row", typeof(CoordinatesFunctionRowItem), typeof(InterpolationComboboxControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(RowChangedCallBack)));

        public CoordinatesFunctionRowItem Row
        {
            get;
            set;
        }

        public InterpolationComboboxControl()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Row != null)
            {
                Row.SelectedInterpolationType = cmb_Interp.SelectedValue.ToString();
            }
        }

        private static void RowChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InterpolationComboboxControl owner = d as InterpolationComboboxControl;
            owner.Row = e.NewValue as CoordinatesFunctionRowItem;
        }
    }
}
