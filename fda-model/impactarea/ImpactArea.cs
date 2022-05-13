using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace impactarea
{
    //TODO
    public class ImpactArea
{
        public string Name { get; set; }
        public int ID { get; set; }

        //we will need to add an index location
        
        public ImpactArea(string name, int id)
        {
            Name = name;
            ID = id;
        }


    }
}
