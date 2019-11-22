using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Serialization
{
    public interface ISerializeToXML<T>
    {
        T Read(string xmlString);
        string WriteToXML();

    }
}
