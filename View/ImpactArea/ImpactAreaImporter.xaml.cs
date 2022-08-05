using HEC.FDA.ViewModel.ImpactArea;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactArea
{
    /// <summary>
    /// Interaction logic for ImpactAreaImporter.xaml
    /// </summary>
    public partial class ImpactAreaImporter : UserControl
    {
        public ImpactAreaImporter()
        {
            InitializeComponent();
        }

        private void Cmb_UniqueName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ImpactAreaImporterVM vm)
            {
                vm.LoadTheRows();
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ImpactAreaImporterVM vm)
            {
                if (!vm.IsCreatingNewElement)
                {
                    row_SelectPath.Height = new GridLength(0);
                    row_SelectUniqueName.Height = new GridLength(0);
                }
                else
                {
                    row_SelectPath.Height = new GridLength(35);
                    row_SelectUniqueName.Height = new GridLength(35);
                }
            }
        }

    }
}
