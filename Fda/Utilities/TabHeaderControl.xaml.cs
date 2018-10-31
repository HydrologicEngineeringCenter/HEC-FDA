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

namespace Fda.Utilities
{
    /// <summary>
    /// Interaction logic for TabHeaderControl.xaml
    /// </summary>
    public partial class TabHeaderControl : UserControl
    {


        public TabHeaderControl()
        {
            InitializeComponent();
        }

        

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {

            FdaViewModel.Utilities.DynamicTabVM vm = (FdaViewModel.Utilities.DynamicTabVM)this.DataContext;
            vm.RemoveTab();

            //DependencyObject currentControl = (DependencyObject)sender;
            //Type targetType = typeof(TabItem);

            //while (currentControl != null)
            //{
            //    if (currentControl.GetType() == targetType)
            //    {
            //        //DynamicTabControl.SelectedItem = currentControl;
            //    }
            //    else
            //    {
            //        currentControl = LogicalTreeHelper.GetParent(currentControl);
            //    }
            //}
            //FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
            ////if (vm.Tabs[DynamicTabControl.SelectedIndex].CanDelete == true)
            //{
            //    //vm.Tabs.RemoveAt(DynamicTabControl.SelectedIndex);
            //}
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Utilities.DynamicTabVM vm = (FdaViewModel.Utilities.DynamicTabVM)this.DataContext;
            vm.PopTabOut();
        }
    }
}
