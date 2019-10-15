using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Validation
{
    public interface IValidator<T>
    {
        bool IsValid(T entity, out IEnumerable<string> errors);
        IEnumerable<string> ReportErrors(T entity);
    }
}
