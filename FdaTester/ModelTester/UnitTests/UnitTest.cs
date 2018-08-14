namespace FdaTester.ModelTester.UnitTests
{
    public abstract class UnitTest
    {
        #region Fields
        protected string _FilePath;
        #endregion

        #region Properties
        public string filePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }
        #endregion

        #region Constructor
        protected UnitTest(string fullFilePath)
        {
            _FilePath = fullFilePath;
        }
        #endregion
    }
}