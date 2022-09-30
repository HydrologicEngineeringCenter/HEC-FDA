using System;

namespace HEC.FDA.Statistics.Distributions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    class StoredAttribute : Attribute
    {
        public string Name;
        public Type type;
    }
}
