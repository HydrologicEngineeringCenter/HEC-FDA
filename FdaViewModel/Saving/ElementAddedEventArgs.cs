using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public class ElementAddedEventArgs:EventArgs
    {

        public BaseFdaElement Element { get; set; }
        public ElementAddedEventArgs(BaseFdaElement element)
        {
            Element = element;
        }

    }

    public class ElementUpdatedEventArgs : EventArgs
    {

        public BaseFdaElement OldElement { get; set; }
        public BaseFdaElement NewElement { get; set; }

        public ElementUpdatedEventArgs(BaseFdaElement oldElement, BaseFdaElement newElement)
        {
            OldElement = oldElement;
            NewElement = newElement;
        }

    }

}
