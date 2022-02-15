using HEC.FDA.ViewModel.Hydraulics;
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
using System.Windows.Shapes;

namespace HEC.FDA.View.Hydraulics
{
    /// <summary>
    /// Interaction logic for GridImporter.xaml
    /// </summary>
    public partial class GridImporter : UserControl
    {
        public GridImporter()
        {
            InitializeComponent();
        }
        public GridImporter(GridImporterVM myVM)
        {
            InitializeComponent();
            Resources["vm"] = myVM;
        }

        private void btn_fileSearch_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Shape files (*.shp)|*.shp|All files (*.*) |*.*";
            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK) //and the filepath is good?
            {
                GridImporterVM tempVM = (GridImporterVM)Resources["vm"];
                tempVM.Path = ofd.FileName;
                Resources["vm"] = tempVM;
            }
        }
    }
}
