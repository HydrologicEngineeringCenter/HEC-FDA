using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for OcctypeErrorsReport.xaml
    /// </summary>
    public partial class OcctypeErrorsReport : UserControl
    {
        public OcctypeErrorsReport()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }

    }
}
