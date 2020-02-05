using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Utilities
{
    public interface ISerializeToXML<T>
    {
        XElement WriteToXML();

    }
}
