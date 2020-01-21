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

namespace View.ImpactArea
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
            FdaViewModel.ImpactArea.ImpactAreaImporterVM vm = (FdaViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
            vm.loadUniqueNames(path);
        }

        private void Cmb_UniqueName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FdaViewModel.ImpactArea.ImpactAreaImporterVM vm = (FdaViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
            vm.LoadTheRows();
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    FdaViewModel.ImpactArea.ImpactAreaImporterVM vm = (FdaViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
        //    if (vm == null) { return; }
        //    if (vm.IsNameReadOnly)
        //    {
        //        row_SelectPath.Height = new GridLength(0);
        //        row_SelectUniqueName.Height = new GridLength(0);
        //    }
        //}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FdaViewModel.ImpactArea.ImpactAreaImporterVM vm = (FdaViewModel.ImpactArea.ImpactAreaImporterVM)this.DataContext;
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
