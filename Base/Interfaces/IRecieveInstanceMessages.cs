using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Interfaces
{
    interface IRecieveInstanceMessages:IRecieveMessages
    {
        int InstanceHash { get; }
    }
}
