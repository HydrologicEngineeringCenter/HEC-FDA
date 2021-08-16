using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides a custom exception for errors generated during the construction of objects.
    /// </summary>
    public class InvalidConstructorArgumentsException: Exception
    {
        /// <summary>
        /// Fatal error messages
        /// </summary>
        IEnumerable<IMessage> Errors { get; }

        /// <summary>
        /// Constructor with no messages, not intended for use.
        /// </summary>
        public InvalidConstructorArgumentsException(): base()
        {
        }
        public InvalidConstructorArgumentsException(IEnumerable<string> errors): base()
        {
            IList<IMessage> messages = new List<IMessage>();
            foreach (var error in errors) messages.Add(new Messages.Message(IMessageLevels.FatalError, error));
            Errors = messages;
        }
        public InvalidConstructorArgumentsException(string message): base(message)
        {
            Errors = new IMessage[] { new Messages.Message(IMessageLevels.FatalError, message) };
        }
        public InvalidConstructorArgumentsException(IEnumerable<IMessage> errors): base()
        {
            Errors = errors;
        }
    }
}
