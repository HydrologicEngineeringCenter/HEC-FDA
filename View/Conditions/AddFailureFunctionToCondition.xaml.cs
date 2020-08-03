using FdaViewModel.Tabs;
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

namespace View.Conditions
{
    /// <summary>
    /// Interaction logic for AddFailureFunctionToCondition.xaml
    /// </summary>
    public partial class AddFailureFunctionToCondition : UserControl
    {
        public AddFailureFunctionToCondition()
        {
            InitializeComponent();
        }

        private void btn_NewFailureFunction_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFailureFunctionToConditionVM vm = (FdaViewModel.Conditions.AddFailureFunctionToConditionVM)this.DataContext;
            vm.LaunchNewFailureFunction(sender, e);
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {

            FdaViewModel.Conditions.AddFailureFunctionToConditionVM vm = (FdaViewModel.Conditions.AddFailureFunctionToConditionVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.OKClicked();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFailureFunctionToConditionVM vm = (FdaViewModel.Conditions.AddFailureFunctionToConditionVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.CancelClicked();

        }


        private void btn_PopOutImporter_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFailureFunctionToConditionVM vm = (FdaViewModel.Conditions.AddFailureFunctionToConditionVM)this.DataContext;
            vm.PopTheImporterOut();
        }



    }
}
