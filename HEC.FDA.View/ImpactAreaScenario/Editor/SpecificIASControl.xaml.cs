using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;

namespace HEC.FDA.View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for SpecificIASControl.xaml
    /// </summary>
    public partial class SpecificIASControl : UserControl
    {

        public SpecificIASControl()
        {
            InitializeComponent();
        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecificIASEditorVM vm = DataContext as SpecificIASEditorVM;
            vm?.Plot();
        }

        private void addThresholdBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecificIASEditorVM vm = DataContext as SpecificIASEditorVM;
            vm?.AddThresholds();
        }
    }
}
