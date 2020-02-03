using HEC.Plotting.SciChart2D.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    internal class ChartStyle
    {
        public SymbolType SymbolType { get; }
        public double SymbolSize { get; }
        public LineStyle LineStyle { get; }
        public double[] StrokeDashArray { get; }

        internal ChartStyle(SciLineData data )
        {
            
            SymbolType = data.SymbolType;
            SymbolSize = data.SymbolSize;
            LineStyle = data.LineStyle;
            StrokeDashArray = data.StrokeDashArray;
        }


    }
}
