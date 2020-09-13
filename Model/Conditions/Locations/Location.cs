using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations
{
    public class Location: ILocation
    {
        public string Name { get; }
        public string Description { get; } = "";

        public Location(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}
