using System;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface INamedAction : INamed
    {
        Action<object, EventArgs> Action { get; set; }
    }
}
