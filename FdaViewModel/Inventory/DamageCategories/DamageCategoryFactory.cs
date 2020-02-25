using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.DamageCategory
{
    public class DamageCategoryFactory
    {
        public static IDamageCategory Factory(string name)
        {
            return new DamageCategory(name);
        }
    }
}
