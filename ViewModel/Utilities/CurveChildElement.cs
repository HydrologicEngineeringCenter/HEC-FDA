using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class CurveChildElement:ChildElement
    {
        protected CurveChildElement(int id) : base(id)
        {
        }

        public ComputeComponentVM ComputeComponentVM { get; set; }

        public override XElement ToXML()
        {
            return null;
        }

    }
}
