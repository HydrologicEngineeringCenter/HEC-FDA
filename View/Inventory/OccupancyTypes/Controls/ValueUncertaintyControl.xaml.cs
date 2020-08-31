using FdaViewModel.Inventory.OccupancyTypes;
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

namespace View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for ValueUncertaintyControl.xaml
    /// </summary>
    public partial class ValueUncertaintyControl : UserControl
    {
        public static readonly DependencyProperty ValueUncertaintyVMProperty = DependencyProperty.Register("ValueUncertaintyVM", typeof(ValueUncertaintyVM), typeof(ValueUncertaintyControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ValueUncertaintyVMChangedCallBack)));

        public ValueUncertaintyVM ValueUncertaintyVM
        {
            get { return (ValueUncertaintyVM)this.GetValue(ValueUncertaintyVMProperty); }
            set { this.SetValue(ValueUncertaintyVMProperty, value); }
        }
        public ValueUncertaintyControl()
        {
            InitializeComponent();
            if (ValueUncertaintyVM != null)
            {
               // cmb_UncertaintyType.ItemsSource = ValueUncertaintyVM.UncertaintyTypes;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            ValueUncertaintyVM vm = (ValueUncertaintyVM)this.DataContext;
            if (e.AddedItems.Count > 0)
            {
                vm.SelectionChanged(e.AddedItems[0]);
                //vm.SelectedDistributionTypeChanged();
            }
        }

        private static void ValueUncertaintyVMChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueUncertaintyControl owner = d as ValueUncertaintyControl;
            ValueUncertaintyVM valueUncertaintyVM = e.NewValue as ValueUncertaintyVM;
            owner.ValueUncertaintyVM = valueUncertaintyVM;
        }

    }
}
