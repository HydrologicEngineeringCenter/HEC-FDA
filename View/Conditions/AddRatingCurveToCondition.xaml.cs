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
    /// Interaction logic for AddRatingCurveToCondition.xaml
    /// </summary>
    public partial class AddRatingCurveToCondition : UserControl
    {
        public AddRatingCurveToCondition()
        {
            InitializeComponent();
        }

        private void btn_NewRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;
            vm.LaunchNewRatingCurve(sender, e);
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;
            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.OKClicked();
            //because i want to set all the linking in the view side and not the viewmodel side, i need to tell the VM that i clicked OK, then i 
            //need to ask the VM if 


            //raise event on parent control (IndividuallinkedplotControl)

           // ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            //((Plots.IndividualLinkedPlotControl)parentControl).UpdateThePlots();
            
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }

            vm.CancelClicked();
            
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;
            vm.PopTheImporterOut();
        }

        private void btn_EditRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;
            vm.EditRatingCurve(sender, e);
        }

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FdaViewModel.Conditions.AddRatingCurveToConditionVM vm = (FdaViewModel.Conditions.AddRatingCurveToConditionVM)this.DataContext;
            vm.UpdateListOfRatingCurves();
        }
    }
}
