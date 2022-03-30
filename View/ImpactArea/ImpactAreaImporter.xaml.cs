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
            cmb_Path.CmbSelectionMade += Cmb_Path_CmbSelectionMade;           
        }
        private void Cmb_Path_CmbSelectionMade(string path)
        {
            ViewModel.ImpactArea.ImpactAreaImporterVM vm = (ViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
            vm.LoadUniqueNames(path);
        }

        private void Cmb_UniqueName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ImpactArea.ImpactAreaImporterVM vm = (ViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
            vm.LoadTheRows();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel.ImpactArea.ImpactAreaImporterVM vm = (ViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
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
