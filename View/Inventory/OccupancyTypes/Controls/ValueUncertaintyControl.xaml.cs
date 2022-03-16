using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes.Controls
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
            //if(typeof(ValueUncertaintyVM).IsAssignableFrom(DataContext.GetType()))
            //{
            //    int i = 0;
            //}

            if (DataContext is ValueUncertaintyVM vm)
            {
                //ValueUncertaintyVM vm = DataContext;
                if (e.AddedItems.Count > 0)
                {
                    vm.SelectionChanged(e.AddedItems[0]);
                    //vm.SelectedDistributionTypeChanged();
                }
            }
        }

        private static void ValueUncertaintyVMChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueUncertaintyControl owner = d as ValueUncertaintyControl;
            ValueUncertaintyVM valueUncertaintyVM = e.NewValue as ValueUncertaintyVM;
            owner.ValueUncertaintyVM = valueUncertaintyVM;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int test = 0;
        }
    }
}
