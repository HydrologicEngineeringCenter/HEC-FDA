﻿using H5Assist.Chunking;
using System;
using System.Reflection;

namespace HEC.FDA.Model.utilities;
internal class Serialization
{
    public static string GetXMLTagFromProperty(Type ownerType, string propertyName)
    {
        var oT = ownerType.GetProperty(propertyName);
        var sP = oT.GetCustomAttribute<StoredPropertyAttribute>();
        return sP.SerializedName;
    }

}
