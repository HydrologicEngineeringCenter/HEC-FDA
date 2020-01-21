using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Inputs.Functions
{
    internal class Ordinate: IValidateData
    {
        #region Properties
        public double X { get; }
        public double Y { get; }
        public bool IsValid { get; }
        #endregion

        #region Constructor
        public Ordinate(double x, double y)
        {
            X = x;
            Y = y;
            IsValid = Validate();
        }
        #endregion

        #region IValidateData Methods
        public bool Validate()
        {
            if (IsFiniteNumber(X) && IsFiniteNumber(Y)) return true;
            else { ReportValidationErrors(); return false; }
        }
        public IEnumerable<string> ReportValidationErrors()
        {
            IList<string> messages = new List<string>();
            if (double.IsNaN(X)) messages.Add("The X value is not a number.");
            if (double.IsNaN(Y)) messages.Add("The Y value is not a number.");
            if (double.IsInfinity(X)) messages.Add("The X value is not a finite number.");
            if (double.IsInfinity(Y)) messages.Add("The Y value is not a finite number.");
            return messages;
        }
        private bool IsFiniteNumber(double value)
        {
            return !double.IsNaN(value) && double.IsInfinity(value);
        }
        #endregion
    }
}
