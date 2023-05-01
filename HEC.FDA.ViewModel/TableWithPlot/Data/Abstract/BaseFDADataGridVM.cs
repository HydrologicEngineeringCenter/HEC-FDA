using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.Abstract
{
    public abstract class BaseFDADataGridVM : IDisplayWithFDADataGrid
    {
        public ObservableCollection<object> Data => throw new NotImplementedException();

        public void AddRow(int i)
        {
            throw new NotImplementedException();
        }

        public void AddUnlinkedRow(int i)
        {
            throw new NotImplementedException();
        }

        public void RemoveRows(List<int> rowIndices)
        {
            throw new NotImplementedException();
        }
    }
}
