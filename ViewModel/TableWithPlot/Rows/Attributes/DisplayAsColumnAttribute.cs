using System;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayAsColumnAttribute : Attribute
    {
        private readonly string _displayName;
        public string DisplayName { get { return _displayName; } }

        public DisplayAsColumnAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}
