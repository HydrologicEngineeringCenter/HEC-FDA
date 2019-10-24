using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public interface IValidate<T>
    {
        bool IsValid { get; }
        IEnumerable<IMessage> Errors { get; }
        bool Validate(IValidator<T> validator, out IEnumerable<IMessage> errors);
    }
}
