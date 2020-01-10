using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// This interface reduces the scope of <see cref="IValidate{T}"/> interface so that objects of multiple types which implement <see cref="IValidate{T}"/> can publish their messages through the <see cref="IMessageSubscriber"/> interface.
    /// </summary>
    /// <remarks> No logic is provided to create <see cref="IMessagePublisher"/>s except through the creation of objects implementing the <see cref="IValidate{T}"/> inferface. </remarks>
    public interface IMessagePublisher
    {
        /// <summary>
        /// A set of <see cref="IMessage"/>s communicating errors and information about the objects that write them.
        /// </summary>
        IEnumerable<IMessage> Messages { get; }
    }
}
