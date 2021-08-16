using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Utilities
{
    public interface ISerializeToXML<T>
    {
        //global::Model.IFdaOrdinate InverseF(global::Model.IFdaOrdinate ordinate);
        //global::Model.IFdaOrdinate InverseF(global::Model.IFdaOrdinate ordinate);
        XElement WriteToXML();

    }
}
