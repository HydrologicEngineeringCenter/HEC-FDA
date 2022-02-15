using System;

namespace ViewModel.TableWithPlot.Rows.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayAsColumnAttribute : Attribute
    {
        private readonly string _displayName;
        private readonly int _displayIndex;
        public string DisplayName { get { return _displayName; } }

        public int DisplayIndex { get { return _displayIndex; } }
        public DisplayAsColumnAttribute(string displayName)
        {
            _displayName = displayName;
            _displayIndex = -1;
        }
        public DisplayAsColumnAttribute(string displayName, int displayIndex )
        {
            _displayName = displayName;
            _displayIndex = displayIndex;
        }
    }
}
