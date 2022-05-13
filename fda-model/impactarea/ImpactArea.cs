using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace impactarea
{
    public class ImpactArea
{
        public string Name { get; set; }
        public int ID { get; set; }        
        public ImpactArea(string name, int id)
        {
            Name = name;
            ID = id;
        }


    }
}
