using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public interface IConverge<T>
    {
        bool IsConverged { get; }
    }
}
