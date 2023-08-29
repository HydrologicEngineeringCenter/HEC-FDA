using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    [Serializable]
    public class ErrorDistribution
    {

        #region Properties
        public ErrorType ErrorType { get; set; }
        public String ErrorTypeCode
        {
            get
            {
                string code = "";

                switch (ErrorType)
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
        }
        public double CentralValue { get; set; }
        public double StandardDeviationOrMin { get; set; }
        public double Maximum { get; set; }
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
            ErrorType = ErrorType.NONE;
            CentralValue = Study.badNumber;
            StandardDeviationOrMin = Study.badNumber;
            Maximum = Study.badNumber;
        }
        public void SetErrorType(string code)
        {
            ErrorType = ErrorType.NONE;
            if (code.ToUpper() == "N")
                ErrorType = ErrorType.NORMAL;
            else if (code.ToUpper() == "LN")
                ErrorType = ErrorType.LOGNORMAL;
            else if (code.ToUpper() == "T")
                ErrorType = ErrorType.TRIANGULAR;
            else
                ErrorType = ErrorType.NONE;
        }

        internal void FixRatioParameters()
        {
            StandardDeviationOrMin = StandardDeviationOrMin * CentralValue;
            Maximum = Maximum * CentralValue;
        }
        #endregion
    }
}
