using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public class FdaValidationResult
    {
        private readonly List<string> _Errors = new List<string>();
        public bool IsValid
        {
            get { return _Errors.Count == 0; }
        }

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
        public void InsertMessage(int index, string message)
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

        /// <summary>
        /// Creates a unique list of error messages. 
        /// </summary>
        /// <returns></returns>
        public string UniqueErrorMessage()
        {
            List<string> errors = _Errors.Distinct().ToList();
            return string.Join(Environment.NewLine, errors);
        }
    }
}
