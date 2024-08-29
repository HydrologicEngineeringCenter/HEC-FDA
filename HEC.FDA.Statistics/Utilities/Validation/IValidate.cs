using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides an interface for data objects which are validated by <see cref="IValidator{T}"/> objects. Follows a visitor pattern.
    /// </summary>
    /// <typeparam name="T"> The type of the object being validated. </typeparam>
    public interface IValidate<T>: IMessagePublisher
    {
        /// <summary>
        /// Calls the data object's <see cref="IValidator{T}"/> to validate the object and return any associated errors and messages.
        /// </summary>
        /// <param name="validator"> The data objects <see cref="IValidator{T}"/>. </param>
        /// <param name="errors"> A set of <see cref="IMessage"/>s filled out during the validation process.</param>
        /// <returns> True if the data object is in a valid state, false otherwise. </returns>
        IMessageLevels Validate(IValidator<T> validator, out IEnumerable<IMessage> errors);     
    }
}
