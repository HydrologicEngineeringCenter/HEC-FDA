using System;
using System.Collections.Generic;
using System.Text;

namespace OccupancyTypes
{
    public static class DamageCategoryFactory
    {
        public static IDamageCategory Factory(string name)
        {
            return new DamageCategory(name);
        }
    }
}
