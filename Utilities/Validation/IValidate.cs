using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Validation
{
    public interface IValidate<T>
    {
        bool IsValid { get; }
        IEnumerable<string> Errors { get; }
        bool Validate(IValidator<T> validator, out IEnumerable<string> errors);
    }
}
