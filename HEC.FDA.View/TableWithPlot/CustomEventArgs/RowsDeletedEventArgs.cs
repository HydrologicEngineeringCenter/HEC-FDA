﻿using System;
using System.Collections.Generic;

namespace HEC.FDA.View.TableWithPlot.CustomEventArgs
{
    public class RowsDeletedEventArgs:EventArgs
    {
        public List<int> RowIndices { get; set; }
        public RowsDeletedEventArgs(List<int> rowIndices)
        {
            RowIndices = rowIndices;
        }

    }
}
