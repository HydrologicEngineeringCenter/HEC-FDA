using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbMemory
{
    [Serializable]
    public class ErrorDistribution
    {
        #region Notes
        // Created By: $username$
        // Created Date: $time$
        //    public enum ErrorTypes { NONE, NORMAL, TRIANGULAR, LOGNORMAL, UNIFORM };
        #endregion
        #region Fields
        private ErrorType _ErrorType;
        private double _CentralValue;
        private double _StdDev;
        private double _Upper;
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public ErrorDistribution()
        {
            Reset();
        }
        #endregion
        #region Voids
        public void Reset()
        {
            _ErrorType = ErrorType.NONE;
            _CentralValue = Study.badNumber;
            _StdDev = Study.badNumber;
            //stdDevLog = 1.0e-7;
            //lower = 0.0;
            _Upper = Study.badNumber;
        }
        #endregion
        #region Functions
        public ErrorType GetErrorType()
        {
            return this._ErrorType;
        }
        public String GetErrorTypeCode()
        {
            string code = "";

            switch (this._ErrorType)
            {
                case ErrorType.NORMAL:
                    code = "N";
                    break;
                case ErrorType.LOGNORMAL:
                    code = "LN";
                    break;
                case ErrorType.TRIANGULAR:
                    code = "T";
                    break;
                default:
                    code = "";
                    break;
            }
            return code;
        }
        public ErrorType SetErrorType(string code)
        {
            _ErrorType = ErrorType.NONE;
            if (code.ToUpper() == "N")
                _ErrorType = ErrorType.NORMAL;
            else if (code.ToUpper() == "LN")
                _ErrorType = ErrorType.LOGNORMAL;
            else if (code.ToUpper() == "T")
                _ErrorType = ErrorType.TRIANGULAR;
            else
                _ErrorType = ErrorType.NONE;
            return _ErrorType;
        }
        public ErrorType SetErrorType(ErrorType errorType)
        {
            this._ErrorType = errorType;
            return this._ErrorType;
        }
        public double GetCentralValue()
        { return _CentralValue; }
        public double SetCentralValue(double centralValue)
        {
            this._CentralValue = centralValue;
            return this._CentralValue;
        }
        public double GetStdDev()
        { return this._StdDev; }
        public double SetStdDev(double stdDev)
        {
            this._StdDev = stdDev;
            return this._StdDev;
        }
        public double GetUpper()
        { return this._Upper; }
        public double SetUpper(double upper)
        {
            this._Upper = upper;
            return this._Upper;
        }
        #endregion
    }
}
