using HEC.FDA.View.TableWithPlot.CustomEventArgs;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using System.Windows.Controls;

namespace HEC.FDA.View.TableWithPlot
{
    /// <summary>
    /// Interaction logic for FdaDataGridControl.xaml
    /// </summary>
    public partial class FdaDataGridControl : UserControl
    {
        public FdaDataGridControl()
        {
            InitializeComponent();
        }

        public void MyDataGrid_PreviewDeleteRows(object sender, PreviewDeleteRowsEventArgs e)
        {
            if ((e.TotalRows - e.RowIndices.Count) < 2) { e.Cancel = true; }
        }

        public void MyDataGrid_RowsDeleted(object sender, RowsDeletedEventArgs e)
        {
            IDataProvider vm = (IDataProvider)DataContext;
            vm.RemoveRows(e.RowIndices);
        }

        public void MyDataGrid_RowsAdded(object sender, RowsAddedEventArgs e)
        {
            IDataProvider vm = (IDataProvider)DataContext;
            for (int i = e.StartRow; i < e.StartRow + e.NumRows; i++)
            {
                vm.AddRow(i);
            }

        }
    }
}
