using System.Windows;
using System.Windows.Controls;
using ViewModel.Tabs;

namespace View.Inventory
{
    /// <summary>
    /// Interaction logic for ImportStructuresFromShapefile.xaml
    /// </summary>
    public partial class ImportStructuresFromShapefile : UserControl
    {
        public ImportStructuresFromShapefile()
        {
            InitializeComponent();
            cmb_Path.CmbSelectionMade += Cmb_Path_CmbSelectionMade;

        }

        private void Cmb_Path_CmbSelectionMade(string path)
        {



            ViewModel.Inventory.ImportStructuresFromShapefileVM vm = (ViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            vm.SelectedPath = path;

            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
            {
                //vm.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                NextButton.IsEnabled = false;
                return;
            }
            NextButton.IsEnabled = true;
            NextButton.Content = "Next→";
            PreviousButton.Visibility = Visibility.Hidden;
            //vm.loadUniqueNames(path);



        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

            ViewModel.Inventory.ImportStructuresFromShapefileVM vm = (ViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            if (vm.NextButtonClicked() == true) //this does validation and returns true if it passes
            {
                stack_ShapefilePath.Visibility = Visibility.Collapsed;
                if (NextButton.Content.ToString() == "Finish")
                {
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
            ViewModel.Inventory.ImportStructuresFromShapefileVM vm = (ViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            vm.PreviousButtonClicked();
            stack_ShapefilePath.Visibility = Visibility.Visible;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //ViewModel.Inventory.ImportStructuresFromShapefileVM vm = (ViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            //vm.RemoveTab();

            RunClosingLogic();



            //vm.CancelButtonClicked();
            //vm.WasCanceled = true;
            //var window = Window.GetWindow(this);
            //window.Close();
        }

    }
}
