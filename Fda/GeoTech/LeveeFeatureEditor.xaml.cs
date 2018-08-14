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

namespace Fda.GeoTech
{
    /// <summary>
    /// Interaction logic for LeveeFeatureEditor.xaml
    /// </summary>
    public partial class LeveeFeatureEditor : UserControl
    {
        public LeveeFeatureEditor()
        {
            InitializeComponent();
         
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FdaViewModel.GeoTech.LeveeFeatureEditorVM vm = (FdaViewModel.GeoTech.LeveeFeatureEditorVM)this.DataContext;
            if (vm.IsInEditMode == true)
            {
                txt_name.IsReadOnly = true;
            }
        }
    }
}
