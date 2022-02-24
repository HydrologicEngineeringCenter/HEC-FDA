using System;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayAsLineAttribute : Attribute
    {
        private readonly string _displayName;
        private readonly Enumerables.ColorEnum _color;
        private readonly bool _dashed;
        public string DisplayName
        {
            get { return _displayName; }    
        }
        public Enumerables.ColorEnum Color
        {
            get { return _color; }
        }
        public bool Dashed
        {
            get { return _dashed; }
        }
        public DisplayAsLineAttribute(string displayname, Enumerables.ColorEnum color = Enumerables.ColorEnum.Black, bool dashed = false )
        {
            _displayName = displayname;
            _color = color;
            _dashed = dashed; 
        }
    }
}
