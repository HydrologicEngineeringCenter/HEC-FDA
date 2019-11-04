using System;
using System.Collections.Generic;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    public interface ITransformFunction<YType>:IFdaFunction<YType>
    {
        ImpactAreaFunctionEnum Type { get; }
        //IComputableTransformFunction Sample(double p);



    }
}
