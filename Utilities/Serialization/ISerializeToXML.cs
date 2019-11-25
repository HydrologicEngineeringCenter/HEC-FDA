using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Utilities.Serialization
{
    public interface ISerializeToXML<T>
    {
        XElement WriteToXML();

    }
}
