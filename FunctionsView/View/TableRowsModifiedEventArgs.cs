using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.View
{
    public class TableRowsModifiedEventArgs
    {
        public List<CoordinatesFunctionRowItem> Rows
        {
            get;
            set;
        }

        public RowModificationEnum ModificationType
        {
            get;
            set;
        }
        public TableRowsModifiedEventArgs(List<CoordinatesFunctionRowItem> rows, RowModificationEnum modType)
        {
            Rows = rows;
            ModificationType = modType;
        }

    }
}
