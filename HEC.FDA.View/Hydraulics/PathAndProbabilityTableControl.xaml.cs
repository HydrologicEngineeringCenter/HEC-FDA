using HEC.FDA.View.TableWithPlot.CustomEventArgs;
using HEC.FDA.ViewModel.Hydraulics;
using System.Windows.Controls;

namespace HEC.FDA.View.Hydraulics
{
    public partial class PathAndProbabilityTableControl : UserControl
    {
        public PathAndProbabilityTableControl()
        {
            InitializeComponent();
        }

        private void MyDataGrid_RowsDeleted(object sender, RowsDeletedEventArgs e)
        {
            if (DataContext is IHaveListOfWSERows vm)
            {
                vm.RemoveRows(e.RowIndices);
            }
        }
    }
}
