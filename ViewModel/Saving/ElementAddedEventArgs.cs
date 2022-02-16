using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Saving
{
    public class ElementAddedEventArgs:EventArgs
    {
        private int _ID = -1;
        public BaseFdaElement Element { get; set; }
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public ElementAddedEventArgs(BaseFdaElement element)
        {
            Element = element;
        }

    }

    public class ElementUpdatedEventArgs : EventArgs
    {
        private int _ID = -1;


        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public BaseFdaElement OldElement { get; set; }
        public BaseFdaElement NewElement { get; set; }

        public ElementUpdatedEventArgs(BaseFdaElement oldElement, BaseFdaElement newElement)
        {
            OldElement = oldElement;
            NewElement = newElement;
            if(newElement is ChildElement)
            {
                ID = ((ChildElement)newElement).GetElementID();
            }
        }

    }

}
