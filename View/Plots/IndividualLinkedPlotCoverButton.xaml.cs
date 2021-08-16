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

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for IndividualLinkedPlotCoverButton.xaml
    /// </summary>
    public partial class IndividualLinkedPlotCoverButton : UserControl
    {
        public IndividualLinkedPlotCoverButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.IndividualLinkedPlotCoverButtonVM vm = (ViewModel.Plots.IndividualLinkedPlotCoverButtonVM)this.DataContext;
            vm.ButtonClicked();
        }
    }
}
