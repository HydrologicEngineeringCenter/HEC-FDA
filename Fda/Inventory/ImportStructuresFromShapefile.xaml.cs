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

namespace Fda.Inventory
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



            FdaViewModel.Inventory.ImportStructuresFromShapefileVM vm = (FdaViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            vm.SelectedPath = path;

            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
            {
                vm.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                NextButton.IsEnabled = false;
                return;
            }
            NextButton.IsEnabled = true;
            NextButton.Content = "Next→";
            PreviousButton.Visibility = Visibility.Hidden;
            vm.loadUniqueNames(path);



        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

            FdaViewModel.Inventory.ImportStructuresFromShapefileVM vm = (FdaViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
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
            FdaViewModel.Inventory.ImportStructuresFromShapefileVM vm = (FdaViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            vm.PreviousButtonClicked();
            stack_ShapefilePath.Visibility = Visibility.Visible;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.ImportStructuresFromShapefileVM vm = (FdaViewModel.Inventory.ImportStructuresFromShapefileVM)this.DataContext;
            //vm.RemoveTab();

            RunClosingLogic();



            //vm.CancelButtonClicked();
            //vm.WasCanceled = true;
            //var window = Window.GetWindow(this);
            //window.Close();
        }
    }
}
