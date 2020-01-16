using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IComputableCondition
    {
        bool CanCompute { get; }
        ICondition ConditionBase { get; }
        void TestCompute();
        void Compute(int seed);
    }
}
