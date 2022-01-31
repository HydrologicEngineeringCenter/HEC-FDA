using System;

namespace Importer
{

    /// <summary>
    /// This class is intended to store and hold on to log information from background threads, and provide asynchronous
    /// access to a buffer of text.
    /// </summary>
    public sealed class AsyncLogger
    {
        private readonly object _mutex = new object();
        private string _buffer = "";

        /// <summary>
        /// Appends a message to the log buffer and appends a line separator after.  This method is synchronized
        /// internally to support asynchronous logging.
        /// </summary>
        /// <param name="message">Message to append</param>
        public void Log(string message)
        {
            lock (_mutex)
            {
                _buffer += message + Environment.NewLine;
            }
        }
        public void Append(string message)
        {
            lock (_mutex)
            {
                _buffer += message;
            }
        }

        /// <summary>
        /// Returns the current message buffer, and clears it.  This method is synchronized internally to support
        /// asynchronous reading and clearing.
        /// </summary>
        /// <returns>Current message buffer</returns>
        public string PopLastMessages()
        {
            string output;

            lock (_mutex)
            {
                output = _buffer;
                //Clear the buffer
                _buffer = "";
            }

            return output;
        }
    }

}

