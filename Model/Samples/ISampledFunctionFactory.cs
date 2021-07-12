using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Samples
{
    public static class ISampledFunctionFactory
    {

        public static ISampledParameter<IFdaFunction> Factory(IFdaFunction function, bool isSampled, double probability)
        {
            ISample sample = null;
            if(isSampled)
            {
                sample = new Sample(probability);
            }
            else
            {
                sample = new Sample();
            }
            return new SampledFunction(sample , function);
        }

    }
}
