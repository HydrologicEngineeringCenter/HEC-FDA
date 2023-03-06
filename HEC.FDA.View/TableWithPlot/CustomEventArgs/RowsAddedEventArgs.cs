using System;

namespace HEC.FDA.View.TableWithPlot.CustomEventArgs
{
    public class RowsAddedEventArgs: EventArgs
    {
        public int StartRow { get; set; }
        public int NumRows { get; set; }
        public RowsAddedEventArgs(int startrow, int numrows )
        {
            StartRow = startrow;
            NumRows = numrows;
        }
    }
}
