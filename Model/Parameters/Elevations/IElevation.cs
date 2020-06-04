using System;
using System.Collections.Generic;
using System.Text;

using Functions;

using Utilities;

namespace Model
{
    /// <summary>
    /// A container for elevation parameters.
    /// </summary>
    public interface IElevation<T>: IParameter<T>, IMessagePublisher
    {
    }
}
