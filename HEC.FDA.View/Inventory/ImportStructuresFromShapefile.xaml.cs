using System.IO;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Tabs;

namespace HEC.FDA.View.Inventory
{
    /// <summary>
    /// Interaction logic for ImportStructuresFromShapefile.xaml
    /// </summary>
    public partial class ImportStructuresFromShapefile : UserControl
    {
        public ImportStructuresFromShapefile()
        {
            InitializeComponent();
        }      

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ImportStructuresFromShapefileVM vm = DataContext as ImportStructuresFromShapefileVM;
            //this does validation and returns true if it passes
            if (vm != null && vm.NextButtonClicked() == true) 
            {
                stack_ShapefilePath.Visibility = Visibility.Collapsed;
                if (NextButton.Content.ToString() == "Finish")
                {
                    vm.WasCanceled = false;
                    RunClosingLogic();
                }
                else
                {
                    NextButton.Content = "Finish";
                    PreviousButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void RunClosingLogic()
        {
            TabController.Instance.CloseTabOrWindow(this);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            NextButton.Visibility = Visibility.Visible;
            NextButton.Content = "Next→";
            PreviousButton.Visibility = Visibility.Collapsed;
            ImportStructuresFromShapefileVM vm = DataContext as ImportStructuresFromShapefileVM;
            vm?.PreviousButtonClicked();
            stack_ShapefilePath.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RunClosingLogic();
        }

    }
}
