using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
public interface IQuartileRowItem
{
    public string Frequency { get; }
    public double Value { get; }

}
