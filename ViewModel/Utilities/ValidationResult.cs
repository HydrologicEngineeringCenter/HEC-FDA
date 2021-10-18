using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationResult(bool isValid = true)
        {
            IsValid = isValid;
        }

        public ValidationResult(bool isValid, string errorMsg)
        {
            IsValid = isValid;
            ErrorMessage = errorMsg;
        }
    }
}
