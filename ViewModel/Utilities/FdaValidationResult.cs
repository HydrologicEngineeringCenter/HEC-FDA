using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class FdaValidationResult
    {
        public bool IsValid
        {
            get { return _Errors.Count == 0; }
        }
        private readonly List<string> _Errors = new List<string>();

        public string ErrorMessage
        {
            get  { return string.Join(Environment.NewLine, _Errors);}
        }

        public FdaValidationResult()
        {
        }

        public FdaValidationResult( string errorMsg)
        {
            _Errors.Add(errorMsg);
        }

        /// <summary>
        /// Does not toggle the IsValid boolean. Inserts the message and adds a newline char.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        public void InsertNewLineMessage(int index, string message)
        {
            _Errors.Insert(index, message);
        }

        public void AddErrorMessages(List<string> errors)
        {
            if (errors.Count > 0)
            {
                _Errors.AddRange(errors);
            }
        }

        public void AddErrorMessage(string msg)
        {
            if (msg != null && msg != "")
            {
                _Errors.Add(msg);
            }
        }
    }
}
