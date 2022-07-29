using HEC.FDA.ViewModel.IndexPoints;
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

namespace HEC.FDA.View.IndexPoints
{
    /// <summary>
    /// Interaction logic for IndexPointsEditor.xaml
    /// </summary>
    public partial class IndexPointsEditor : UserControl
    {
        public IndexPointsEditor()
        {
            InitializeComponent();
        }

        private void Cmb_UniqueName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is IndexPointsEditorVM vm)
            {
                vm.LoadTheRows();
            }
        }   

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IndexPointsEditorVM vm && !vm.IsCreatingNewElement)
            {
                row_SelectUniqueName.Height = new GridLength(0);
            }
        }
    }
}
