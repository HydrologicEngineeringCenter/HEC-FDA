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
}
