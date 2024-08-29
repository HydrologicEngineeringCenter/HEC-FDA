using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public interface IValidator<T>
    {
        IMessageLevels IsValid(T entity, out IEnumerable<IMessage> errors);
        IEnumerable<IMessage> ReportErrors(T entity);
    }
}
