using System;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.AlternativeComparisonReport;

namespace HEC.FDA.View.Alternatives
{
    /// <summary>
    /// Interaction logic for CreateNewAlternative.xaml
    /// </summary>
    public partial class CreateNewAlternative : UserControl
    {
        public CreateNewAlternative()
        {
            InitializeComponent();
        }
       
        private void btn_addIncrement_Click(object sender, RoutedEventArgs e)
        {
            CreateNewAlternativeComparisonReportVM vm = DataContext as CreateNewAlternativeComparisonReportVM;
            vm?.AddComparison();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewAlternativeComparisonReportVM vm = DataContext as CreateNewAlternativeComparisonReportVM;
            vm?.RemoveSelectedRow();
        }
    }
}
