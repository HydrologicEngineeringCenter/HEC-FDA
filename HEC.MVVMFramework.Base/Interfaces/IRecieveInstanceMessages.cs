using System.Collections.Generic;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IRecieveInstanceMessages : IRecieveMessages
    {
        List<int> InstanceHash { get; set; } 
    }
}
