using System;

namespace HEC.FDA.Model.utilities;

[AttributeUsage(AttributeTargets.All, Inherited = true)]
sealed class StoredPropertyAttribute : Attribute
{
    readonly string _serializedName;
    public string[] AlsoKnownAs { get; set; }
    public StoredPropertyAttribute(string serializedName)
    {
        _serializedName = serializedName;
    }
    public string SerializedName
    {
        get { return _serializedName; }
    }
}
