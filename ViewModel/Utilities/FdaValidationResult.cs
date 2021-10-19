using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class FdaValidationResult
    {
        public bool IsValid { get; set; }
        public StringBuilder ErrorMessage { get; set; } = new StringBuilder();

        public FdaValidationResult(bool isValid = true)
        {
            IsValid = isValid;
        }

        public FdaValidationResult(bool isValid, string errorMsg)
        {
            IsValid = isValid;
            ErrorMessage.Append(errorMsg);
        }

        /// <summary>
        /// Does not toggle the IsValid boolean. Inserts the message and adds a newline char.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        public void InsertNewLineMessage(int index, string message)
        {
            ErrorMessage.Insert(index, message + Environment.NewLine);
        }

        public void AddValidationResult(FdaValidationResult result)
        {
            if(!result.IsValid)
            {
                IsValid = false;
                ErrorMessage.AppendLine(result.ErrorMessage.ToString());
            }
        }
    }
}
