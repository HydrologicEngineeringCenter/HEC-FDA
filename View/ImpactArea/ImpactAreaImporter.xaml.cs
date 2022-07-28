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
            ImpactAreaImporterVM vm = (ImpactAreaImporterVM)this.DataContext;
            vm.LoadTheRows();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ImpactAreaImporterVM vm = (ImpactAreaImporterVM)this.DataContext;
            if (vm == null) { return; }
            if (vm.IsInEditMode)
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
