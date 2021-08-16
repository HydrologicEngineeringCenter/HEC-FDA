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
    /// Interaction logic for OccTypeGroupRowItem.xaml
    /// </summary>
    public partial class OccTypeGroupRowItem : UserControl
    {

        private string _Name;
        private ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement _OccupancyTypeGroup;
        //public string Path
        //{
        //    get { return _Path; }
        //    set { _Path = value; }
        //}
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement OccupancyTypeGroup
        {
            get { return _OccupancyTypeGroup; }
            set { _OccupancyTypeGroup = value; }
        }

        public OccTypeGroupRowItem()
        {
            InitializeComponent();
        }

        public OccTypeGroupRowItem(string path, string name)
        {
            //InitializeComponent();

            //txt_Path.Text = System.IO.Path.GetFileName(path);
            //txt_Name.Text = name;
            //_Name = name;
            ////create occtype group element
            //Consequences_Assist.ComputableObjects.OccupancyTypes ots = new Consequences_Assist.ComputableObjects.OccupancyTypes(path);
            //_OccupancyTypeGroup = new ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement(name, ots.OccupancyTypes);
            //txt_NumOcctypes.Text =  ots.OccupancyTypes.Count.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM vm = (ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM)this.DataContext;
            vm.UpdateTheRows();

            //foreach (ViewModel.Inventory.OccupancyTypes.OccupancyTypesGroupRowItemVM row in vm.ListOfRowVMs)
            //{
            //    if (Name == row.Name)
            //    {
            //        vm.ListOfRowVMs.Remove(row);

            //        break;
            //    }
            //}

            //if (this.UpdateTheListOfRows != null)
            //{
            //    this.UpdateTheListOfRows(this, new EventArgs());
            //}

        }

       

      
    }
}
