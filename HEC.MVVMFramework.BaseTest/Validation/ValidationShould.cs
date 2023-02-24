using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading.Tasks;

namespace BaseTest.Validation
{
    [Trait("RunsOn", "Remote")]
    public class ValidationShould
    {
        /// <summary>
        /// This test is here so that I can put a break point on the last line and see what the string message looks like.
        /// </summary>
        [Fact]
        public void TestGetErrorMessages()
        {
            TestObject obj = new TestObject();
            obj.Validate();
            String errorMessage = obj.GetErrorMessages();

            Assert.True(true);
        }

    }
}
