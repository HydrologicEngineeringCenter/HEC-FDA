using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using System.Windows.Controls;

namespace HEC.FDA.View.WaterSurfaceElevation
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
