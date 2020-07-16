using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations
{
    internal class Location: ILocation
    {
        public string Name { get; }
        public string Description { get; } = "";

        internal Location(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}
