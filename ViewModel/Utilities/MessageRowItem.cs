using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class MessageRowItem
    {
        public string Date { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public string LogLevel { get; set; }
        public string Logger { get; set; }
        public MessageRowItem(string date, string message, string user, string logLevel, string logger)
        {
            LogLevel = logLevel;
            Logger = logger;
            Date = date;
            Message = message;
            User = user;
        }
    }
}
