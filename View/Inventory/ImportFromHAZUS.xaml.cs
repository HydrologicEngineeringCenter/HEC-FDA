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

namespace Fda.Inventory
{
    /// <summary>
    /// Interaction logic for ImportFromHAZUS.xaml
    /// </summary>
    public partial class ImportFromHAZUS : UserControl
    {
        public ImportFromHAZUS()
        {
            InitializeComponent();
        }

        private void BndryGbsButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "bndrygbs (*.mdb)|*.mdb|All files (*.*) |*.*";
            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK) //and the filepath is good?
            {
                FdaViewModel.Inventory.ImportFromHAZUSVM tempVM = (FdaViewModel.Inventory.ImportFromHAZUSVM)Resources["vm"];
                tempVM.BndryGbsPath = ofd.FileName.ToString();

                if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(ofd.FileName.ToString()) + "\\flVeh.mdb"))
                {
                  tempVM.FlVehPath   = System.IO.Path.GetDirectoryName(ofd.FileName.ToString()) + "\\flVeh.mdb";
                }
                if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(ofd.FileName.ToString()) + "\\MSH.mdb"))
                {
                    tempVM.MSHPath = System.IO.Path.GetDirectoryName(ofd.FileName.ToString()) + "\\MSH.mdb";
                }

                
                Resources["vm"] = tempVM;
            }
        }

        private void flVehButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "flVeh (*.mdb)|*.mdb|All files (*.*) |*.*";
            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK) //and the filepath is good?
            {
                flVehTextBox.Text = ofd.FileName.ToString();
                FdaViewModel.Inventory.ImportFromHAZUSVM tempVM = (FdaViewModel.Inventory.ImportFromHAZUSVM)Resources["vm"];
                tempVM.FlVehPath = flVehTextBox.Text;
                Resources["vm"] = tempVM;
            }
        }

        private void MSHButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "MSH (*.mdb)|*.mdb|All files (*.*) |*.*";
            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK) //and the filepath is good?
            {
                MSHTextBox.Text = ofd.FileName.ToString();
                FdaViewModel.Inventory.ImportFromHAZUSVM tempVM = (FdaViewModel.Inventory.ImportFromHAZUSVM)Resources["vm"];
                tempVM.MSHPath = MSHTextBox.Text;
                Resources["vm"] = tempVM;
            }
        }

       
    }
}
