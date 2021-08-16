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
    /// Interaction logic for OccupancyTypesFromFile.xaml
    /// </summary>
    public partial class OccupancyTypesFromFile : UserControl
    {
        public OccupancyTypesFromFile()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Inventory.AttributeLinkingListVM vm = (ViewModel.Inventory.AttributeLinkingListVM)this.DataContext;
            if (vm == null)
            {
            }
            else
            {
                for (int i = 0; i < vm.OccupancyTypesInFile.Count; i++)
                {
                    AddRow(vm.OccupancyTypesInFile[i], vm.OccupancyTypesInStudy);

                }
            }
        }


        private void AddRow(string columnOneText, List<string> columnTwoList)
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(23);

            LinkingGrid.RowDefinitions.Add(newRow);
            TextBlock column1 = new TextBlock();
            column1.Text = columnOneText;
            Grid.SetRow(column1, LinkingGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(column1, 0);
            LinkingGrid.Children.Add(column1);

            ComboBox column2 = new ComboBox();
            column2.ItemsSource = columnTwoList;
            Grid.SetRow(column2, LinkingGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(column2, 1);
            column2.SelectedIndex = -1;
            LinkingGrid.Children.Add(column2);

        }




    }
}
