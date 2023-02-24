using HEC.MVVMFramework.Base.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseTest.Validation
{
    public class TestObject : HEC.MVVMFramework.Base.Implementations.Validation
    {
        public int TestProperty { get; set; } = 100;
        public int TestProperty2 { get; set; } = 100;

        public TestObject()
        {
            AddSinglePropertyRule(nameof(TestProperty), new Rule(() => TestProperty < 100, "Test property is in error. Do better next time.", HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(TestProperty2), new Rule(() => TestProperty2 < 100, "Another property is in error. Keep trying. Never give up, never surrender.", HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Major));

        }

    }
}
