using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class CurveChildElement:ChildElement
    {
        public const string CHILD_ELEMENT = "ChildElement";
        public ComputeComponentVM ComputeComponentVM { get; set; }

        protected CurveChildElement(string name, string lastEditDate, string description, ComputeComponentVM compVM, int id) : base(name, lastEditDate, description, id)
        {
            ComputeComponentVM = compVM;
        }

        protected CurveChildElement(XElement childElem, int id) : base(childElem, id)
        {
            ReadHeaderXElement(childElem.Element(HEADER_XML_TAG));
            XElement functionElem = childElem.Element("ComputeComponentVM");
            ComputeComponentVM = new ComputeComponentVM(functionElem);
        }


        public override XElement ToXML()
        {
            XElement childElem = new XElement(CHILD_ELEMENT);
            childElem.Add(CreateHeaderElement());
            childElem.Add(ComputeComponentVM.ToXML());
            return childElem;
        }

        public bool Equals(CurveChildElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }

            //computeComponentVM doesn't have an equals method. I am just going to compare the selected item for now
            if(!ComputeComponentVM.SelectedItemToPairedData().Equals(elem.ComputeComponentVM.SelectedItemToPairedData()))
            {
                isEqual = false;
            }

            return isEqual;
        }

    }
}
