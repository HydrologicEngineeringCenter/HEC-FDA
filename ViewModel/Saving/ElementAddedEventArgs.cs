using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Saving
{
    public class ElementAddedEventArgs:EventArgs
    {
        public ChildElement Element { get; set; }

        public ElementAddedEventArgs(ChildElement element)
        {
            Element = element;
        }
    }

    public class ElementUpdatedEventArgs : EventArgs
    {        
        public ChildElement NewElement { get; set; }

        public ElementUpdatedEventArgs(ChildElement newElement)
        {
            NewElement = newElement;
        }

    }

}
