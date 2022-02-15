using HEC.FDA.ViewModel.AlternativeComparisonReport;
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

namespace HEC.FDA.View.Alternatives
{
    /// <summary>
    /// Interaction logic for IncrementRowItem.xaml
    /// </summary>
    public partial class IncrementRowItem : UserControl
    {
        public event EventHandler DeleteThisRow;
        public static readonly DependencyProperty IncrementToBindWithProperty = DependencyProperty.Register("IncrementToBindWith", typeof(Increment), typeof(IncrementRowItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(IncrementToBindWithChangedCallBack)));

        public Increment IncrementToBindWith
        {
            get { return (Increment)this.GetValue(IncrementToBindWithProperty); }
            set { this.SetValue(IncrementToBindWithProperty, value); }
        }
        public IncrementRowItem(int index, bool isFirstRow = false)
        {
            InitializeComponent();
            if(isFirstRow)
            {
                btn_delete.Visibility = Visibility.Hidden;
               
            }
            lbl_IncrementName.Content = "Increment " + index + ":";
        }

        public void SetSpecialBindingForFirstRow(Increment inc)
        {
            //set the first row binding of the second combobox to the vm
            Binding myBinding = new Binding();
            myBinding.Source = inc;
            myBinding.Path = new PropertyPath("SelectedPlan2");
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(cmb_Plans2, ComboBox.SelectedValueProperty, myBinding);
        }


        public void SetBindingForSecondCombobox()
        {
            Binding myBinding = new Binding();
            myBinding.Source = IncrementToBindWith;
            myBinding.Path = new PropertyPath("SelectedPlan1");
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            cmb_Plans2.Visibility = Visibility.Hidden;
            txt_plan2.Visibility = Visibility.Visible;
            BindingOperations.SetBinding(txt_plan2, TextBlock.TextProperty, myBinding);
        }

        private static void IncrementToBindWithChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IncrementRowItem owner = d as IncrementRowItem;
            Increment inc = e.NewValue as Increment;
            owner.IncrementToBindWith = inc;
            owner.SetBindingForSecondCombobox();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteThisRow?.Invoke(this, e);
        }
    }

}
