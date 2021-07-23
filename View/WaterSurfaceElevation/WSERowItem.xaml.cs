using ViewModel.WaterSurfaceElevation;
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

namespace View.WaterSurfaceElevation
{
    /// <summary>
    /// Interaction logic for WSERowItem.xaml
    /// </summary>
    public partial class WSERowItem : UserControl
    {


        public WSERowItem(WaterSurfaceElevationElement element)
        {
            InitializeComponent();
            txt_name.Text = element.Name;
            txt_desc.Text = element.Description;
            lbl_number.Content = "Number of Elevations: " + element.RelativePathAndProbability.Count;
        }
    }
}
