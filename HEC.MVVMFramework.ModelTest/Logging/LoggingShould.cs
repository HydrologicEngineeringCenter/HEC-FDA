using BaseTest.Validation;
using HEC.MVVMFramework.Model.Messaging;
using System;
using System.Collections.Generic;
using Xunit;

namespace ModelTest.Logging
{
    [Trait("RunsOn", "Remote")]
    public class LoggingShould
    {
        [Fact]
        public void TestLoggingMessage()
        {
            ValidationGroup vg = CreateTestGroup1();
            string msg = vg.GetErrorMessages();

            int i = 0;
        }

        [Fact]
        public void TestLoggingMessageWith2Groups()
        {
            ValidationGroup vg = CreateTestGroup1();
            ValidationGroup vg2 = CreateTestGroup2();
            vg.ChildGroups.Add(vg2);

            string msg = vg.GetErrorMessages();
            int i = 0;
        }

        [Fact]
        public void TestLoggingMessageWith3Groups()
        {
            ValidationGroup vg = CreateTestGroup1();
            ValidationGroup vg2 = CreateTestGroup2();
            ValidationGroup vg3 = CreateTestGroup3();

            vg2.ChildGroups.Add(vg3);
            vg.ChildGroups.Add(vg2);

            string msg = vg.GetErrorMessages();
            int i = 0;
        }


        private ValidationGroup CreateTestGroup1()
        {
            LoggingObject objA = new LoggingObject("A");
            LoggingObject objB = new LoggingObject("B");

            objA.Validate();
            objB.Validate();

            List<ValidationErrorLogger> loggingObjs = new List<ValidationErrorLogger>() { objA, objB };
            List<string> introMessages = new List<string>()
            {
                "Errors with objA:",
                "Errors with objB:",
            };
            return new ValidationGroup(loggingObjs, introMessages, "Test group 1 has errors:");
        }

        private ValidationGroup CreateTestGroup2()
        {
           
            LoggingObject objC = new LoggingObject("C");
            LoggingObject objD = new LoggingObject("D");
            LoggingObject objE = new LoggingObject("E");

            
            objC.Validate();
            objD.Validate();
            objE.Validate();

            List<ValidationErrorLogger> loggingObjs = new List<ValidationErrorLogger>() { objC, objD, objE };
            List<string> introMessages = new List<string>()
            {
                "Errors with objC:",
                "Errors with objD:",
                "Errors with objE:"
            };
            return new ValidationGroup(loggingObjs, introMessages, "Test group 2 has errors:");
        }

        private ValidationGroup CreateTestGroup3()
        {

            LoggingObject objF = new LoggingObject("F");
            LoggingObject objG = new LoggingObject("G");
            LoggingObject objH = new LoggingObject("H");


            objF.Validate();
            objG.Validate();
            objH.Validate();

            List<ValidationErrorLogger> loggingObjs = new List<ValidationErrorLogger>() { objF, objG, objH };
            List<string> introMessages = new List<string>()
            {
                "Errors with objF:",
                "Errors with objG:",
                "Errors with objH:"
            };
            return new ValidationGroup(loggingObjs, introMessages, "Test group 3 has errors:");
        }
    }
}
