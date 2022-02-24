using System;

namespace HEC.FDA.View.TableWithPlot.CustomEventArgs
{
    public class PreviewLastRowTabEventArgs : EventArgs
    {
        public int CellIndex { get; set; }    
        public PreviewLastRowTabEventArgs(int cellIndex)
        {
            CellIndex = cellIndex;  
        }
    }
}
