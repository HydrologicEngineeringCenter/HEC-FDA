using HEC.FDA.ViewModel.TableWithPlot;

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
