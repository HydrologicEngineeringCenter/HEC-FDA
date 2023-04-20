using HEC.FDA.ViewModel.Results;
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

namespace HEC.FDA.View.Results
{
    /// <summary>
    /// Interaction logic for ScenarioDamageSummary.xaml
    /// </summary>
    public partial class ScenarioDamageSummary : UserControl
    {
        public ScenarioDamageSummary()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //todo: this is an attempt to get the column headers correct on the middle table. It is not working.
            //delete if this message is still here.
            int i = 0;
            foreach(DataGridColumn col in damCatGrid.Columns)
            {
                col.SetValue(FrameworkElement.NameProperty, "col" + i);
                i++;
            }

        }

        private void damCatGrid_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
        }
    }
}
