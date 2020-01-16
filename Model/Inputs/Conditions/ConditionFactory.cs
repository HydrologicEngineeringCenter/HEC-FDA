using Model.Inputs.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public static class ConditionFactory
    {
        public static ICondition Factory(string name, int year, IFrequencyFunction frequencyFunction, 
            IList<ITransformFunction> transformFunctions, IList<IMetric> metrics)
        {
            return new Inputs.Conditions.Condition(name, year, frequencyFunction, transformFunctions, metrics);
        }
    }
}
