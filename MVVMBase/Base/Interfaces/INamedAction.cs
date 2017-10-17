using System;

namespace Base.Interfaces
{
    public interface INamedAction : INamed
    {
        Action<object, EventArgs> Action { get; set; }
    }
}
