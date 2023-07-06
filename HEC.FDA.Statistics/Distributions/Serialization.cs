using System;
using System.Reflection;

namespace HEC.FDA.Model.utilities;
internal class Serialization
{
    public static string GetXMLTagFromProperty(Type ownerType, string propertyName)
    {
        return ownerType.GetProperty(propertyName).GetCustomAttribute<StoredPropertyAttribute>().SerializedName;
    }
}
