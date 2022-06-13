using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for ComputeScenarioEditor.xaml
    /// </summary>
    public partial class ComputeScenarioEditor : UserControl
    {
        public ComputeScenarioEditor()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }
    }
}
