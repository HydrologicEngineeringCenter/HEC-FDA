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

namespace Fda.Output
{
    /// <summary>
    /// Interaction logic for LinkedPlotsView.xaml
    /// </summary>
    public partial class LinkedPlotsUpdated : UserControl
    {
        public LinkedPlotsUpdated()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Output.LinkedPlotsUpdatedVM vm = (ViewModel.Output.LinkedPlotsUpdatedVM)DataContext;
            //OxyPlot1

        }
    }
}
