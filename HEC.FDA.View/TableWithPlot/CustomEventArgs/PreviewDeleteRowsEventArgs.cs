using System;
using System.Collections.Generic;

namespace HEC.FDA.View.TableWithPlot.CustomEventArgs
{

        public class PreviewDeleteRowsEventArgs : EventArgs
        {
            public List<int> RowIndices { get; set; }
            public  bool Cancel { get; set; }
            public int TotalRows { get; set; }
            public PreviewDeleteRowsEventArgs(List<int> rowIndices, int totalRows, bool cancel)
            {
                RowIndices = rowIndices;
                Cancel = cancel;
                TotalRows = totalRows;
            }
        }
    

}
