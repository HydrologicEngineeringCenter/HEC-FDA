using Importer;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for ImportOccupancyTypeGroups.xaml
    /// </summary>
    public partial class ImportOccupancyTypeGroups : UserControl
    {
        private List<OccTypeGroupRowItem> _ListOfRows;
        public List<OccTypeGroupRowItem> ListOfRows
        {
            get { return _ListOfRows; }
            set { _ListOfRows = value; }
        }

        public ImportOccupancyTypeGroups()
        {
            InitializeComponent();
            ListOfRows = new List<OccTypeGroupRowItem>();
        }

        public void DeleteSelectedOcctypeGroup(object sender,EventArgs e)
        {
            DrawAllTheRows();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_Path.Text == null || cmb_Path.Text == "") { return; }

            ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;
            vm.SelectedPath = cmb_Path.Text;

            AsyncLogger logger = new AsyncLogger();
            AsciiImport import = new AsciiImport(logger);
            //the importer will read the file and load the occtype property with any occtypes it found
            import.ImportAsciiData(vm.SelectedPath, AsciiImport.ImportOptions.ImportOcctypes);
            DrawAllTheRows();
            cmb_Path.Text = "";
            vm.SelectedPath = "";
        }

        public void RemoveAndReDrawTheRows(object sender, EventArgs e)
        {
            //remove the sender row from the list
            ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;
            ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM sendingRow = sender as ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM;
            if(sendingRow == null) { return; }
            vm.ListOfRowVMs.Remove(sendingRow);
            DrawAllTheRows();
        }
        public void DrawAllTheRows()
        {
            //first clear everything that was in the grid
            grd_rows.Children.Clear();
            grd_rows.RowDefinitions.Clear();
            grd_rows.ColumnDefinitions.Clear();

            ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM vm = (ViewModel.Inventory.OccupancyTypes.ImportOccupancyTypesVM)this.DataContext;

            foreach(ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM row in vm.ListOfRowVMs)
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
