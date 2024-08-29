using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Utilities
{
    public interface ISerializeToXML<T>
    {
        [Obsolete("Write To XML from the ISerializeToXML<T> remove, it is a generic interface with no generic methods")]
        XElement WriteToXML();
        //T ReadFromXML(XElement ele);
    }
}
