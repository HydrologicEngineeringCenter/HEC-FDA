using System.Windows;
using System.Windows.Controls;
using ViewModel.Inventory;
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

        //Todo: Ideally this code wouldn't be necessary. The path selection control should be able to bind to the view model
        //correctly. That is more work than i want to deal with so i am leaving this code here for now. - cody 12/8/21
        private void Cmb_Path_CmbSelectionMade(string path)
        {
            ImportStructuresFromShapefileVM vm = DataContext as ImportStructuresFromShapefileVM;
            vm.SelectedPath = path;

            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
            {
                NextButton.IsEnabled = false;
                return;
            }
            NextButton.IsEnabled = true;
            NextButton.Content = "Next→";
            PreviousButton.Visibility = Visibility.Hidden;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ImportStructuresFromShapefileVM vm = DataContext as ImportStructuresFromShapefileVM;
            //this does validation and returns true if it passes
            if (vm.NextButtonClicked() == true) 
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
            ImportStructuresFromShapefileVM vm = DataContext as ImportStructuresFromShapefileVM;
            vm.PreviousButtonClicked();
            stack_ShapefilePath.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RunClosingLogic();
        }
    }
}
