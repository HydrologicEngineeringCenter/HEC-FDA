using FdaModel.Utilities.Attributes;

namespace FdaModel.Utilities.Messager
{
    [Author("William Lehman", "05/31/2016")]
    public class ErrorMessage
    {
        #region Notes
        //This class is intended to allow any error found in the view, model or view model to be reported with its severity.
        #endregion

        #region Fields
        private ErrorMessageEnum _Errorlevel;
        private string _ErrorMessage;
        private string _reportedBy;
        private string _dateTime;
        private string _userName;
        #endregion

        #region Properties
        public string Message
        {
            get { return _ErrorMessage; }
        }
        public ErrorMessageEnum ErrorLevel
        {
            get { return _Errorlevel; }
        }
        public string ReportedFrom
        {
            get { return _reportedBy; }
        }
        public string User
        {
            get { return _userName; }
        }
        public string Date
        {
            get { return _dateTime; }
        }
        #endregion

        #region Constructor
        public ErrorMessage(string message, ErrorMessageEnum logLevel, [System.Runtime.CompilerServices.CallerFilePath] string originatorType = "")
        {
            _ErrorMessage = message;
            _Errorlevel = logLevel;
            if (System.IO.Path.GetExtension(originatorType) == ".cs")
            {
                _reportedBy = System.IO.Path.GetFileNameWithoutExtension(originatorType);
            }
            else
            {
                _reportedBy = originatorType;
            }
            _dateTime = System.DateTime.Now.ToString();
            _userName = System.Environment.UserName;
        }
        #endregion
    }
}
