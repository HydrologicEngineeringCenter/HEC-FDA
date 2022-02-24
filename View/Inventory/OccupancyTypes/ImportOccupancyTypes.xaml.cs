using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for ImportOccupancyTypes.xaml
    /// </summary>
    public partial class ImportOccupancyTypes : UserControl
    {
        public ImportOccupancyTypes()
        {
            InitializeComponent();
           // cmb_Path.CmbSelectionMade += Cmb_Path_CmbSelectionMade;
           
        }
        private void cmb_Path_SelectionMade(string fullpath, string filename)
        {
            HEC.FDA.ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (HEC.FDA.ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;
            vm.SelectedPath = fullpath;
            lbl_PassFail.Content = "";
            if (txt_Name.Text == null || txt_Name.Text == "")
            {
                txt_Name.Text = System.IO.Path.GetFileNameWithoutExtension(fullpath);
            }
        }
        //private void Cmb_Path_CmbSelectionMade(string path)
        //{
        //    HEC.FDA.ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (HEC.FDA.ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;
        //    vm.SelectedPath = path;
        //    if(txt_Name.Text == null || txt_Name.Text == "")
        //    {
        //        txt_Name.Text = System.IO.Path.GetFileNameWithoutExtension(path);
        //    }
        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //todo: i don't think this class is being used anywhere. Delete?
            //ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (HEC.FDA.ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;

 

            //vm.OccupancyTypesGroupName = txt_Name.Text;
            //if (vm.Import() == true) //this lets you know if the import was successful
            //{
            //    lbl_PassFail.Content = "Imported " + txt_Name.Text + " Successfully!";
            //    cmb_Path.Path = "";
            //    txt_Name.Text = "";

            //}
            //else
            //{
            //    lbl_PassFail.Content = "Import of " + txt_Name.Text + " Failed!";
            //}
        }

       
    }
}
