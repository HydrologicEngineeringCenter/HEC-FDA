using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

using Utilities.Ranges;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UtilitiesTests.Ranges
{
    [ExcludeFromCodeCoverage]
    public class RangeDoubleTests
    {

        [Theory]
        [InlineData(0d, 1d, true, true, true, true, 0d)]
        [InlineData(0d, 1d, true, false, true, true, 0d)]
        [InlineData(0d, 1d, true, true, false, true, 0d)]
        [InlineData(0d, 1d, true, true, true, false, 0d)]
        [InlineData(0d, 1d, false, true, true, true, 0d + double.Epsilon)]
        [InlineData(0d, 1d, false, false, true, true, 0d + double.Epsilon)]
        [InlineData(0d, 1d, false, false, false, true, 0d + double.Epsilon)]
        [InlineData(0d, 1d, false, false, false, false, 0d + double.Epsilon)]
        public void Min_InclusiveMinTrueOrFalse_Returns_ExpectedMin(double min, double max, bool inclusiveMin, bool inclusiveMax, bool finiteReq, bool notSingleValReq, double expected)
        {
            var testObj = new RangeDouble(min, max, inclusiveMin, inclusiveMax, finiteReq, notSingleValReq);
            Assert.Equal(expected, testObj.Min);
        }
        [Theory]
        [InlineData(0d, 0d)]
        public void Messages_MinEqualsMaxAllowed_Returns_ExpectedMessage(double min, double max)
        {           
            var testObj = new RangeDouble(min, max, true, true, true, false);
            bool pass = testObj.Messages.Count() == 1 
                && testObj.Messages.First().Level == Utilities.IMessageLevels.Message 
                && testObj.Messages.First().Notice == $"The range minimum and maximum values: {min} are identical. This is allowed but makes results in a range that contains a single point.";
            Assert.True(pass);
        }
    }
}
