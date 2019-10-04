using System;

namespace FdaModel.Utilities.Attributes
{
    [Author("William Lehman", "06/08/2016", "06/10/2016")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    class Tested : Attribute
    {
        #region Fields
        private bool _IsTested;
        private bool _TestPassed;
        private string _TestFilesPath;
        private string _TestedDate;
        private string _Tester;
        #endregion

        #region Constructor        
        [Tested(false)]
        /// <summary>
        /// Use only when testing has not begun.
        /// Creates a test status attribute to document if a constructor, function, void or other method (e.g. target) has been tested, if the target passed those tests, where the test files are located, when the test was conducted and who conducted the test.
        /// </summary>
        /// <param name="isTested"> Should always be false if this constructor is being used. </param>
        public Tested(bool isTested)
        {
            _IsTested = isTested;
            _TestPassed = false;
            _TestFilesPath = null;
            _TestedDate = null;
            _Tester = null;
        }

        [Tested(false)]
        /// <summary>
        /// Use only when testing has begun.
        /// Creates a test status attribute to document if a constructor, function, void or other method (e.g. target) has been tested, if the target passed those tests, where the test files are located, when the test was conducted and who conducted the test.
        /// </summary>
        /// <param name="isTested"> This should always be true if this constructor is being used. </param>
        /// <param name="testPassed"> Status of test. </param>
        /// <param name="testFilePath"> Location of test files. </param>
        /// <param name="testDate"> Date most recent tests were performed. </param>
        /// <param name="testerName"> Name of individual that performed the tests. </param>
        public Tested(bool isTested, bool testPassed, string testFilePath, string testDate, string testerName)
        {
            _IsTested = isTested;
            _TestPassed = testPassed;
            _TestFilesPath = testFilePath;
            _TestedDate = testDate;
            _Tester = testerName;
        }
        #endregion
    }
}

