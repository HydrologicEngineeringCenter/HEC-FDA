namespace HEC.FDA.ViewModel.Utilities
{
    public class MessageItem
    {
        #region Fields
        private string _Errorlevel;
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
        public string ErrorLevel
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
        public MessageItem(string message, string logLevel, string originatorType, string dateTime, string userName)
        {
            _ErrorMessage = message;
            _Errorlevel = logLevel;
            _reportedBy = originatorType;
            _dateTime = dateTime;
            _userName = userName;
        }
        #endregion
    }
}
