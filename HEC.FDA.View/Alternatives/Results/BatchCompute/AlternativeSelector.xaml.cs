using HEC.FDA.ViewModel.Alternatives.Results.BatchCompute;
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

namespace HEC.FDA.View.Alternatives.Results.BatchCompute
{
    /// <summary>
    /// Interaction logic for AlternativeSelector.xaml
    /// </summary>
    public partial class AlternativeSelector : UserControl
    {
        public AlternativeSelector()
        {
            InitializeComponent();
        }

        private void Compute_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AlternativesSelectorVM vm)
            {
                vm.ComputeClicked();
            }
        }
    }
}
