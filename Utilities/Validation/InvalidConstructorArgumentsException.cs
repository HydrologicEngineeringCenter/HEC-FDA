using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Validation
{
    public class InvalidConstructorArgumentsException: Exception
    {
        IEnumerable<string> Errors { get; }

        public InvalidConstructorArgumentsException(): base()
        {
        }
        public InvalidConstructorArgumentsException(IEnumerable<string> messages): base()
        {
            Errors = messages;
        }
        public InvalidConstructorArgumentsException(string message): base(message)
        {
            Errors = new string[] { message };
        }

    }
}
