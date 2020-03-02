using FdaViewModel.Inventory.OccupancyTypes;
using Importer;
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

namespace View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for ImportOccupancyTypeGroups.xaml
    /// </summary>
    public partial class ImportOccupancyTypeGroups : UserControl
    {

        //private List<FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesElement> _OccTypeGroupsList;
        private List<OccTypeGroupRowItem> _ListOfRows;
        public List<OccTypeGroupRowItem> ListOfRows
        {
            get { return _ListOfRows; }
            set { _ListOfRows = value; }
        }
        //public List<FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesElement> OccTypeGroupsList
        //{
        //    get { return _OccTypeGroupsList; }
        //    set { _OccTypeGroupsList = value; }
        //}

        public ImportOccupancyTypeGroups()
        {
            InitializeComponent();
            //OccTypeGroupsList = new List<FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesElement>();
            ListOfRows = new List<OccTypeGroupRowItem>();
        }

        //private void cmb_Path_SelectionMade(string fullpath, string filename)
        //{

        //}

        public void DeleteSelectedOcctypeGroup(object sender,EventArgs e)
        {

           

            
            DrawAllTheRows();

            //OccTypeGroupRowItem ri = (OccTypeGroupRowItem)sender;
            //for(int i = 0; i<ListOfRows.Count;i++)
            //{
            //    if(ListOfRows[i].Name == ri.Name)
            //    {
            //        ListOfRows.RemoveAt(i);

                    //        foreach (UIElement control in grd_rows.Children)
                    //        {
                    //            var usercontrol = control as OccTypeGroupRowItem;
                    //            if (usercontrol != null)
                    //            {
                    //                int childRowIndex = (int)usercontrol.GetValue(Grid.RowProperty);
                    //                if (childRowIndex - 1 == i)
                    //                {
                    //                    grd_rows.Children.Remove(control);
                    //                    grd_rows.RowDefinitions.RemoveAt(childRowIndex);

                    //                    break;

                    //                }
                    //            }
                    //        }
                    //        //OccupancyTypeGroupsGrid.RowDefinitions[i + 1].IsEnabled = false;
                    //        //OccupancyTypeGroupsGrid.Children.RemoveAt(i + 1);
                    //       // OccupancyTypeGroupsGrid.RowDefinitions[i+1].Height = new GridLength(0); // have to do + 1 because the first row in the grid is the column titles
                    //    }
                    //}

        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_Path.Text == null || cmb_Path.Text == "") { return; }

            FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;
            vm.SelectedPath = cmb_Path.Text;

            AsciiImport import = new AsciiImport();
            //the importer will read the file and load the occtype property with any occtypes it found
            import.ImportAsciiData(vm.SelectedPath, AsciiImport.ImportOptions.ImportOcctypesOnly);
            //vm.Import(import.OccupancyTypes);

            if (vm.Import(import.OccupancyTypes) == true)//this lets you know if the import was successful. the row was now added to the vm's list of rows
            {
                DrawAllTheRows();
               

                //{

                //    FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM row = vm.ListOfRowVMs.Last();

                    

                //    RowDefinition newRow = new RowDefinition();
                //    newRow.Height = new GridLength(27);

                //    grd_rows.RowDefinitions.Add(newRow);

                //    ContentControl cc = new ContentControl();
                    

                //    Grid.SetRow(cc, grd_rows.RowDefinitions.Count - 1);
                //    Grid.SetColumn(cc, 0);
                //    Grid.SetColumnSpan(cc, 3);

                //    cc.Content = row;


                //    grd_rows.Children.Add(cc);
                //}

            }

            cmb_Path.Text = "";
            vm.SelectedPath = "";


        }

        public void RemoveAndReDrawTheRows(object sender, EventArgs e)
        {
            //remove the sender row from the list

            FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;

            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM sendingRow = sender as FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM;
            if(sendingRow == null) { return; }

            vm.ListOfRowVMs.Remove(sendingRow);
            //foreach (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM row in vm.ListOfRowVMs)
            //{
            //    if (sendingRow.Name == row.Name)
            //    {
            //        vm.ListOfRowVMs.Remove(row);

            //        break;
            //    }
            //}
            DrawAllTheRows();
        }
        public void DrawAllTheRows()
        {
            //first clear everything that was in the grid
            grd_rows.Children.Clear();
            grd_rows.RowDefinitions.Clear();
            grd_rows.ColumnDefinitions.Clear();


            FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (FdaViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;

            foreach(FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM row in vm.ListOfRowVMs)
            {
                row.UpdateTheListOfRows += new EventHandler(RemoveAndReDrawTheRows);
                RowDefinition newRow = new RowDefinition();
                newRow.Height = new GridLength(31);
                
                grd_rows.RowDefinitions.Add(newRow);

                ContentControl cc = new ContentControl();


                Grid.SetRow(cc, grd_rows.RowDefinitions.Count - 1);
                Grid.SetColumn(cc, 0);
                Grid.SetColumnSpan(cc, 4);

                cc.Content = row;
                


                grd_rows.Children.Add(cc);
            }
        }

    }
}
