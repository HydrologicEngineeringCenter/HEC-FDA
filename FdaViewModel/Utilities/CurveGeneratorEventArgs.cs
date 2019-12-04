using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FdaViewModel.CurveGeneratorVM;

namespace FdaViewModel.Utilities
{
    public class CurveGeneratorEventArgs:EventArgs
    {

        public CurveGeneratorRowItem ChangedRow { get; }
        public List<CurveGeneratorRowItem> RowsBelowChangedRow { get; }
        public List<CurveGeneratorRowItem> RowsAboveChangedRow { get; }

        public CurveGeneratorEventArgs(CurveGeneratorRowItem changedRow, List<CurveGeneratorRowItem> rowsBelowChangedRow, List<CurveGeneratorRowItem> rowsAboveChangedRow)
        {
            ChangedRow = changedRow;
            RowsBelowChangedRow = rowsBelowChangedRow;
            RowsAboveChangedRow = rowsAboveChangedRow;
        }
    }
}
