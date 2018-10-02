using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class MessageRowItem
    {
        public string Date { get; set; }
        public string Message { get; set; }
        public string User { get; set; }

        public MessageRowItem(string date, string message, string user)
        {
            Date = date;
            Message = message;
            User = user;
        }
    }
}
