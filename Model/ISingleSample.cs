using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface ISingleSample<Input, Output>
    {
        int InputRandomNumberCount(Input input);
        Output SingleSample(Input input, double[] packet);
    }
}
