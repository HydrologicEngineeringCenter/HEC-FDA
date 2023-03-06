using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class CurveChildElement:ChildElement
    {
        public const string CHILD_ELEMENT = "ChildElement";
        public CurveComponentVM CurveComponentVM { get; set; }

        protected CurveChildElement(string name, string lastEditDate, string description, CurveComponentVM compVM, int id) : base(name, lastEditDate, description, id)
        {
            CurveComponentVM = compVM;
        }

        protected CurveChildElement(XElement childElem, int id) : base(childElem, id)
        {
            ReadHeaderXElement(childElem.Element(HEADER_XML_TAG));
            CurveComponentVM = CurveComponentVM.CreateCurveComponentVM(childElem);  
        }


        public override XElement ToXML()
        {
            XElement childElem = new XElement(CHILD_ELEMENT);
            childElem.Add(CreateHeaderElement());
            childElem.Add(CurveComponentVM.ToXML());
            return childElem;
        }

        public bool Equals(CurveChildElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }

            //todo: CurveComponentVM doesn't have an equals method. I am just going to compare the selected item for now
            if(!CurveComponentVM.SelectedItemToPairedData().Equals(elem.CurveComponentVM.SelectedItemToPairedData()))
            {
                isEqual = false;
            }

            return isEqual;
        }

    }
}
