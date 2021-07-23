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

namespace View.Inventory
{
    /// <summary>
    /// Interaction logic for ImportFromNSI.xaml
    /// </summary>
    public partial class ImportFromNSI : UserControl
    {
        public ImportFromNSI()
        {
            InitializeComponent();
        }


        //this click event handles both buttons by checking the sender name
        public void studyAreaButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Shape files (*.shp)|*.shp|All files (*.*) |*.*";

            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();
 
            if (dr == System.Windows.Forms.DialogResult.OK) //and the filepath is good?
            {
                ViewModel.Inventory.ImportFromNSIVM tempVM = (ViewModel.Inventory.ImportFromNSIVM)Resources["vm"];

                if (((Button)sender).Name == "studyAreaButton")
                {
                    tempVM.StudyAreaPath = ofd.FileName;
                }
                else { tempVM.UserDefinedPath = ofd.FileName; }
                
                Resources["vm"] = tempVM;
            }
        }


    }
}
