using HEC.FDA.ViewModel.TableWithPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class CurveChildElement:ChildElement
    {
        protected CurveChildElement(int id) : base(id)
        {
        }

        public ComputeComponentVM ComputeComponentVM { get; set; }

    }
}
