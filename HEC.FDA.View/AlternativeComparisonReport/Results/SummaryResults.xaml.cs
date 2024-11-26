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

namespace HEC.FDA.View.AlternativeComparisonReport.Results
{
    /// <summary>
    /// Interaction logic for SummaryResults.xaml
    /// </summary>
    public partial class SummaryResults : UserControl
    {
        public SummaryResults()
        {
            InitializeComponent();
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "C2";
            }

        }
    }
}
