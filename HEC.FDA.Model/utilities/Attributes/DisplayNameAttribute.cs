using System;

namespace HEC.FDA.Model.utilities.Attributes;
internal class DisplayNameAttribute: Attribute
{
    public string DisplayName { get; private set; }
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }


}
